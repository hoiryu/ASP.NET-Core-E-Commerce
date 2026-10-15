# PuppeteerSharp로 PDF 생성하기

> 상태: **가이드만 있음. 아직 코드에 적용하지 않음** (2026-10-08 작성)
> 설치부터 "사용자 목록 PDF 다운로드" API를 만드는 것까지 순서대로 따라 할 수 있게 정리한 문서입니다.

---

## 1. 전체 흐름

클라이언트는 **HTML이 아니라 조회 조건만** 보냅니다. HTML은 서버가 Razor 템플릿으로 직접 만듭니다.

```
클라이언트 ── GET /api/users/pdf?paging.take=50 ──▶ UsersController
                                                     │ 1. IUsersService 로 데이터 조회
                                                     │ 2. Razor 컴포넌트(UsersReport.razor)로 HTML 생성
                                                     │ 3. IPdfGenerator(PuppeteerSharp)로 HTML → PDF
클라이언트 ◀──────── application/pdf 파일 ───────────┘
```

### 왜 클라이언트에서 HTML을 받지 않는가

| 문제 | 설명 |
|---|---|
| SSRF | 서버의 Chrome이 받은 HTML을 그대로 렌더링하므로, `<img src="http://내부서버/...">` 같은 태그로 **서버가 내부망에 요청을 보내게** 만들 수 있음 |
| 위변조 | 금액, 이름 등을 클라이언트가 마음대로 바꾼 HTML로 "서버가 발행한" PDF를 만들 수 있음 |
| 일관성 | 서버 데이터와 서버 템플릿으로 만들어야 누가 요청하든 같은 결과가 나옴 |

---

## 2. 만들 파일

현재 계층 구조(`PersonManager.Api → PersonManager.Services → PersonManager.ServiceContracts → PersonManager.Entities`)에 맞춰 나눕니다.

| 파일 | 프로젝트 | 역할 |
|---|---|---|
| `PersonManager.ServiceContracts/Common/Pdf/IPdfGenerator.cs` | PersonManager.ServiceContracts | HTML → PDF 변환 인터페이스 |
| `PersonManager.Services/Common/Pdf/PuppeteerPdfGenerator.cs` | PersonManager.Services | PuppeteerSharp로 구현 |
| `PersonManager.Api/Common/Templates/RazorTemplateRenderer.cs` | Api | Razor 컴포넌트 → HTML 문자열 |
| `PersonManager.Api/Templates/_Imports.razor` | Api | 템플릿 공통 `@using` |
| `PersonManager.Api/Templates/Users/UsersReport.razor` | Api | 사용자 목록 PDF 템플릿 |
| `PersonManager.Api/Controllers/Modules/Identity/Users/UsersController.cs` | Api | `GET api/users/pdf` 액션 추가 (수정) |
| `PersonManager.Api/Program.cs` | Api | DI 등록 (수정) |

> Razor 템플릿은 Api 프로젝트에 둡니다. Api는 `Microsoft.NET.Sdk.Web`이라 `.razor` 파일과 `HtmlRenderer`를 **추가 패키지 없이** 쓸 수 있습니다.

---

## 3. 설치

PuppeteerSharp는 **PersonManager.Services 프로젝트에만** 설치합니다.

```bash
dotnet add PersonManager.Services/PersonManager.Services.csproj package PuppeteerSharp
```

설치 후 `PersonManager.Services/PersonManager.Services.csproj`에 아래 항목이 추가됩니다. (2026-10-08 기준 최신 안정 버전은 `25.12.0`)

```xml
<ItemGroup>
  <PackageReference Include="PuppeteerSharp" Version="25.12.0" />
</ItemGroup>
```

> Chrome은 따로 설치하지 않아도 됩니다. 첫 PDF 요청 때 `BrowserFetcher`가 자동으로 내려받습니다(약 150MB, 1회).

---

## 4. 코드 적용

### 4-1. 인터페이스 — `PersonManager.ServiceContracts/Common/Pdf/IPdfGenerator.cs`

```csharp
namespace PersonManager.ServiceContracts.Common.Pdf;

/// <summary>
/// HTML 을 PDF 로 변환
/// </summary>
public interface IPdfGenerator
{
	Task<byte[]> GeneratePdfFromHtml(string html);
}
```

### 4-2. 구현 — `PersonManager.Services/Common/Pdf/PuppeteerPdfGenerator.cs`

```csharp
using PuppeteerSharp;
using PuppeteerSharp.Media;
using PersonManager.ServiceContracts.Common.Pdf;

namespace PersonManager.Services.Common.Pdf;

/// <summary>
/// HTML 을 headless Chrome 으로 렌더링해서 PDF 로 변환
/// 브라우저 실행은 비싸므로 처음 요청 때 한 번만 띄우고 재사용함 (Singleton 으로 등록할 것)
/// </summary>
public sealed class PuppeteerPdfGenerator : IPdfGenerator, IAsyncDisposable
{
	// Chrome 을 내려받을 위치 (bin 폴더 아래라 git 에 올라가지 않음, clean 하면 다시 내려받음)
	private static readonly string ChromePath = Path.Combine(AppContext.BaseDirectory, ".chrome");

	private readonly SemaphoreSlim _launchLock = new(1, 1);
	private IBrowser? _browser;

	public async Task<byte[]> GeneratePdfFromHtml(string html)
	{
		IBrowser browser = await GetBrowser();

		// 요청마다 새 탭을 열고, 끝나면 닫음 (브라우저는 공유)
		await using IPage page = await browser.NewPageAsync();
		await page.SetJavaScriptEnabledAsync(false); // 템플릿에 JS 가 없으므로 꺼 둠
		await page.SetContentAsync(html);

		return await page.PdfDataAsync(
			new PdfOptions
			{
				Format = PaperFormat.A4,
				PrintBackground = true, // CSS 배경색 출력
				MarginOptions = new MarginOptions
				{
					Top = "15mm",
					Bottom = "15mm",
					Left = "10mm",
					Right = "10mm",
				},
				DisplayHeaderFooter = true,
				HeaderTemplate = "<div></div>",
				FooterTemplate =
					"<div style=\"width:100%; font-size:9px; text-align:center;\"><span class=\"pageNumber\"></span> / <span class=\"totalPages\"></span></div>",
			}
		);
	}

	private async Task<IBrowser> GetBrowser()
	{
		if (_browser is { IsClosed: false })
			return _browser;

		// 동시에 여러 요청이 와도 브라우저는 하나만 띄우도록 잠금
		await _launchLock.WaitAsync();
		try
		{
			if (_browser is { IsClosed: false })
				return _browser;

			// 이미 내려받았으면 다시 받지 않음
			var browserFetcher = new BrowserFetcher(new BrowserFetcherOptions { Path = ChromePath });
			InstalledBrowser installedBrowser = await browserFetcher.DownloadAsync();

			_browser = await Puppeteer.LaunchAsync(
				new LaunchOptions { Headless = true, ExecutablePath = installedBrowser.GetExecutablePath() }
			);

			return _browser;
		}
		finally
		{
			_launchLock.Release();
		}
	}

	// 앱 종료 시 DI 컨테이너가 호출 → Chrome 프로세스 정리
	public async ValueTask DisposeAsync()
	{
		if (_browser is not null)
			await _browser.DisposeAsync();

		_launchLock.Dispose();
	}
}
```

### 4-3. Razor 렌더러 — `PersonManager.Api/Common/Templates/RazorTemplateRenderer.cs`

.NET 8부터 기본 제공되는 `HtmlRenderer`로 Razor 컴포넌트를 HTML 문자열로 만듭니다.

```csharp
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace PersonManager.Api.Common.Templates;

/// <summary>
/// Razor 컴포넌트(.razor)를 HTML 문자열로 렌더링
/// </summary>
public class RazorTemplateRenderer(IServiceProvider _serviceProvider, ILoggerFactory _loggerFactory)
{
	public async Task<string> Render<TComponent>(Dictionary<string, object?> parameters)
		where TComponent : IComponent
	{
		await using var htmlRenderer = new HtmlRenderer(_serviceProvider, _loggerFactory);

		// HtmlRenderer 호출은 반드시 Dispatcher 안에서 실행해야 함
		return await htmlRenderer.Dispatcher.InvokeAsync(async () =>
		{
			ParameterView parameterView = ParameterView.FromDictionary(parameters);
			var output = await htmlRenderer.RenderComponentAsync<TComponent>(parameterView);

			return output.ToHtmlString();
		});
	}
}
```

### 4-4. 템플릿 공통 using — `PersonManager.Api/Templates/_Imports.razor`

```razor
@using Microsoft.AspNetCore.Components
@using PersonManager.ServiceContracts.Modules.Identity.Users.Dtos
```

### 4-5. 템플릿 — `PersonManager.Api/Templates/Users/UsersReport.razor`

일반 HTML + CSS로 작성합니다. `@user.Name`처럼 출력하는 값은 **Razor가 자동으로 HTML 인코딩**하므로 데이터에 `<script>`가 들어 있어도 태그로 실행되지 않습니다.

```razor
<!DOCTYPE html>
<html lang="ko">
<head>
	<meta charset="utf-8" />
	<style>
		body { font-family: "Noto Sans KR", "Apple SD Gothic Neo", "Malgun Gothic", sans-serif; font-size: 11px; color: #222; }
		h1 { font-size: 18px; margin: 0 0 4px; }
		.meta { color: #666; margin: 0 0 12px; }
		table { width: 100%; border-collapse: collapse; }
		th, td { border: 1px solid #ccc; padding: 4px 6px; text-align: left; }
		th { background: #f2f4f7; }
		thead { display: table-header-group; } /* 페이지가 넘어가도 헤더 반복 */
		tr { page-break-inside: avoid; } /* 행이 페이지 경계에서 잘리지 않게 */
	</style>
</head>
<body>
	<h1>사용자 목록</h1>
	<p class="meta">생성일시: @GeneratedAt.ToString("yyyy-MM-dd HH:mm") · 총 @Users.Count 명</p>

	<table>
		<thead>
			<tr>
				<th>이름</th>
				<th>이메일</th>
				<th>생년월일</th>
				<th>성별</th>
				<th>국가</th>
				<th>주소</th>
				<th>뉴스레터</th>
			</tr>
		</thead>
		<tbody>
			@foreach (UserResponse user in Users)
			{
				<tr>
					<td>@user.Name</td>
					<td>@user.Email</td>
					<td>@user.DateOfBirth?.ToString("yyyy-MM-dd")</td>
					<td>@user.Gender</td>
					<td>@user.Country?.Name</td>
					<td>@user.Address</td>
					<td>@(user.ReceiveNewsLetters ? "Y" : "N")</td>
				</tr>
			}
		</tbody>
	</table>
</body>
</html>

@code {
	[Parameter, EditorRequired]
	public List<UserResponse> Users { get; set; } = [];

	[Parameter]
	public DateTime GeneratedAt { get; set; } = DateTime.Now;
}
```

> CSS 안에 `@media`, `@page`처럼 `@`로 시작하는 규칙을 쓰려면 Razor 문법과 겹치므로 `@@media`, `@@page`로 적어야 합니다.

### 4-6. DI 등록 — `PersonManager.Api/Program.cs`

`#region Register with IoC Container` 안에 추가합니다.

```csharp
using PersonManager.Api.Common.Templates;
using PersonManager.ServiceContracts.Common.Pdf;
using PersonManager.Services.Common.Pdf;

// ...

#region Register with IoC Container
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddSingleton<IPdfGenerator, PuppeteerPdfGenerator>(); // 브라우저를 공유해야 하므로 Singleton
builder.Services.AddScoped<RazorTemplateRenderer>();
#endregion
```

> `PuppeteerPdfGenerator`는 **반드시 Singleton**으로 등록합니다. Scoped로 등록하면 요청마다 Chrome을 새로 띄웠다 닫아서 매우 느려집니다.

### 4-7. 컨트롤러 — `PersonManager.Api/Controllers/Modules/Identity/Users/UsersController.cs`

생성자에 의존성 2개를 추가하고, 기존 `GetUsers`와 같은 조회 조건을 받는 액션을 만듭니다.

```csharp
using Microsoft.AspNetCore.Mvc;
using PersonManager.Api.Common.Templates;
using PersonManager.Api.Templates.Users;
using PersonManager.ServiceContracts.Common.Dtos;
using PersonManager.ServiceContracts.Common.Pdf;
using PersonManager.ServiceContracts.Modules.Identity.Users;
using PersonManager.ServiceContracts.Modules.Identity.Users.Dtos;

namespace PersonManager.Api.Controllers.Modules.Identity.Users
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController(
		IUsersService _usersService,
		IPdfGenerator _pdfGenerator,
		RazorTemplateRenderer _templateRenderer
	) : ControllerBase
	{
		// ... 기존 액션 그대로 ...

		[HttpGet("pdf")]
		public async Task<IActionResult> GetUsersPdf(
			[FromQuery(Name = "filter")] UserFilter filter,
			[FromQuery(Name = "order")] UserOrder order,
			[FromQuery(Name = "paging")] Paging paging
		)
		{
			List<UserResponse> users = await _usersService.GetUsers(filter, order, paging);

			string html = await _templateRenderer.Render<UsersReport>(
				new Dictionary<string, object?>
				{
					[nameof(UsersReport.Users)] = users,
					[nameof(UsersReport.GeneratedAt)] = DateTime.Now,
				}
			);

			byte[] pdf = await _pdfGenerator.GeneratePdfFromHtml(html);

			return File(pdf, "application/pdf", $"users-{DateTime.Now:yyyyMMddHHmmss}.pdf");
		}
	}
}
```

> 컴포넌트의 네임스페이스는 폴더 경로를 따라 `PersonManager.Api.Templates.Users`가 됩니다.

---

## 5. 실행과 확인

```bash
dotnet run --project PersonManager.Api
```

다른 터미널에서 호출합니다.

```bash
# 전체
curl -o users.pdf "http://localhost:4000/api/users/pdf"

# 조건 지정 (기존 GET /api/users 와 같은 쿼리 형식)
curl -o users.pdf "http://localhost:4000/api/users/pdf?filter.gender=Male&order.name=Asc&paging.take=50"

open users.pdf
```

- **첫 요청은 느립니다.** Chrome을 내려받고 실행하기 때문입니다. 두 번째 요청부터는 빠릅니다.
- 브라우저에서 `http://localhost:4000/api/users/pdf`를 열어도 PDF가 다운로드됩니다.

---

## 6. 선택: 앱 시작 시 미리 준비하기

첫 요청이 느린 게 싫다면 `Program.cs`에서 `app.Run()` 직전에 Chrome을 미리 내려받아 둡니다. 이 코드는 다운로드만 하고 실행은 하지 않습니다.

```csharp
await new BrowserFetcher(
	new BrowserFetcherOptions { Path = Path.Combine(AppContext.BaseDirectory, ".chrome") }
).DownloadAsync();

app.Run();
```

---

## 7. Linux / Docker로 배포할 때

macOS 로컬에서는 바로 동작하지만, Linux 컨테이너에서는 다음 두 가지를 챙겨야 합니다.

1. **Chrome 실행에 필요한 라이브러리**와 **한글 폰트**를 설치합니다. 폰트가 없으면 한글이 `□□□`로 나옵니다.

   ```dockerfile
   FROM mcr.microsoft.com/dotnet/aspnet:10.0
   RUN apt-get update && apt-get install -y --no-install-recommends \
       chromium fonts-noto-cjk \
       && rm -rf /var/lib/apt/lists/*
   ```

2. 컨테이너에서는 내려받는 대신 **시스템에 설치한 chromium**을 쓰고, 샌드박스를 끕니다. `GetBrowser()`의 `LaunchAsync` 부분을 이렇게 바꿉니다.

   ```csharp
   _browser = await Puppeteer.LaunchAsync(
   	new LaunchOptions
   	{
   		Headless = true,
   		ExecutablePath = "/usr/bin/chromium",
   		Args = ["--no-sandbox", "--disable-dev-shm-usage"],
   	}
   );
   ```

   > `--no-sandbox`는 컨테이너가 root로 실행될 때 필요합니다. 이 경우 **신뢰할 수 있는 HTML만 렌더링해야** 합니다. 이 가이드처럼 서버 템플릿만 쓰면 괜찮습니다.

실제로는 환경마다 경로가 다르므로 `appsettings.json`에 `Pdf:ChromeExecutablePath` 같은 설정을 두고, 값이 있으면 그 경로를 쓰고 없으면 `BrowserFetcher`로 내려받게 만드는 것을 추천합니다.

---

## 8. 자주 겪는 문제

| 증상 | 원인 / 해결 |
|---|---|
| 첫 요청이 수십 초 걸림 | Chrome 다운로드 중. 6번처럼 미리 받아 두기 |
| 한글이 `□□□`로 나옴 | 서버에 한글 폰트 없음. `fonts-noto-cjk` 설치 |
| 배경색이 안 나옴 | `PdfOptions.PrintBackground = true` 확인 |
| 표 헤더가 첫 페이지에만 나옴 | `thead { display: table-header-group; }` |
| 행이 페이지 경계에서 잘림 | `tr { page-break-inside: avoid; }` |
| 요청할 때마다 느림 | `PuppeteerPdfGenerator`를 Scoped/Transient로 등록함. Singleton으로 변경 |
| 앱 종료 후 Chrome 프로세스가 남음 | 강제 종료(kill)하면 `DisposeAsync`가 호출되지 않음. 정상 종료(Ctrl+C) 사용 |
| Linux에서 `Failed to launch browser` | Chrome 의존 라이브러리 누락 또는 sandbox 문제. 7번 참고 |

---

## 9. 적용 체크리스트

- [ ] `dotnet add PersonManager.Services/PersonManager.Services.csproj package PuppeteerSharp`
- [ ] `IPdfGenerator` 추가 (PersonManager.ServiceContracts)
- [ ] `PuppeteerPdfGenerator` 추가 (PersonManager.Services)
- [ ] `RazorTemplateRenderer` 추가 (Api)
- [ ] `Templates/_Imports.razor`, `Templates/Users/UsersReport.razor` 추가 (Api)
- [ ] `Program.cs`에 DI 등록 (`IPdfGenerator`는 Singleton)
- [ ] `UsersController`에 `GetUsersPdf` 액션 추가
- [ ] `curl -o users.pdf http://localhost:4000/api/users/pdf`로 확인

---

## 참고

- PuppeteerSharp 문서: https://www.puppeteersharp.com/docs/index.html
- Razor 컴포넌트를 HTML로 렌더링 (`HtmlRenderer`): https://learn.microsoft.com/aspnet/core/blazor/components/render-components-outside-of-aspnetcore
