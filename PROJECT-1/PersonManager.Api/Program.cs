using System.Text.Json.Serialization;
using Gridify;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PersonManager.Api.Common.ExceptionHandlers;
using PersonManager.Api.Endpoints.Modules.Identity.Countries;
using PersonManager.Api.Endpoints.Modules.Identity.Users;
using PersonManager.Entities.Data;
using PersonManager.Repositories;
using PersonManager.Services;
using Serilog;

// 1단계: 호스트 구성 전에 쓰는 임시 로거 (호스트 구성 중 예외 기록용)
// 호스트가 아직 없으므로 appsettings 를 직접 읽어 파일 저장 설정을 2단계와 공유
var bootstrapConfiguration = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory()) // 호스트의 ContentRoot 와 같은 위치
	.AddJsonFile("appsettings.json", optional: true)
	.AddJsonFile(
		$"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
		optional: true
	)
	.Build();

Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(bootstrapConfiguration)
	.WriteTo.Console()
	.CreateBootstrapLogger();

try
{
	var builder = WebApplication.CreateBuilder(args);

	#region JSON
	builder.Services.ConfigureHttpJsonOptions(options =>
		options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()) // Enum 을 문자열로 직렬화
	);
	#endregion

	#region Validation
	// Endpoint 매개변수의 DataAnnotations 를 핸들러 실행 전에 검사하고, 실패하면 400 ValidationProblem 반환
	builder.Services.AddValidation();
	#endregion

	#region Exception Handling
	// 오류 응답을 ProblemDetails(RFC 9457) 형식으로 통일
	builder.Services.AddProblemDetails();
	// ArgumentException → 400, 유니크 인덱스 위반 → 409 (그 외 예외는 500)
	builder.Services.AddExceptionHandler<ApiExceptionHandler>();
	#endregion

	#region EndPoint 를 kabab-case 로 변환
	builder.Services.AddRouting(o => o.LowercaseUrls = true);
	#endregion

	// builder.Services.AddAppOptions(builder.Configuration); // Options 패턴 바인딩 + 검증

	#region Register with IoC Container
	builder.Services.AddRepositories(); // Repository + Gridify 매퍼
	builder.Services.AddServices(); // 서비스 (로깅 인터셉터 프록시)
	#endregion

	#region Gridify
	GridifyGlobalConfiguration.EnableEntityFrameworkCompatibilityLayer(); // EF Core 에서 필터 값을 SQL 파라미터로 전달
	#endregion

	#region Register DbContext
	builder.Services.AddDbContext<AppDbContext>(options =>
		options
			.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
			.UseSnakeCaseNamingConvention()
			// 실패 예외는 위로 전파되어 ExceptionHandler 가 처리·기록하므로 EF 자체 에러 로그는 Debug 로 낮춤
			.ConfigureWarnings(w =>
				w.Log((RelationalEventId.CommandError, LogLevel.Debug), (CoreEventId.SaveChangesFailed, LogLevel.Debug))
			)
	);
	#endregion

	#region Serilog
	// 2단계: appsettings·DI 를 반영한 최종 로거로 임시 로거를 대체
	builder.Services.AddSerilog(
		(services, lc) =>
			lc
				.ReadFrom.Configuration(builder.Configuration) // appsettings 의 "Serilog" 섹션 (최소 레벨)
				.ReadFrom.Services(services)
				.Enrich.FromLogContext()
				.WriteTo.Console()
	);
	#endregion

	var app = builder.Build();

	app.UseStaticFiles(); // Static Files
	// UseStaticFiles 보다 뒤에서 처리해서 Static File 로그는 제외
	// 요청당 한 줄: 메서드, 경로+쿼리, 상태 코드, 소요 시간
	app.UseSerilogRequestLogging(o =>
	{
		o.IncludeQueryInRequestPath = true; // 모든 요청 완료 로그에 클라이언트 정보 추가
		o.EnrichDiagnosticContext = (diag, http) =>
		{
			diag.Set("ClientIp", http.Connection.RemoteIpAddress?.ToString());
			diag.Set("UserAgent", http.Request.Headers.UserAgent.ToString());
		};
	});
	// 요청 로그보다 안쪽에 둬야 요청 로그에 변환된 상태 코드(400)가 기록된다
	app.UseExceptionHandler();
	app.UseRouting(); // Routing 매칭 지점 표시
	// 매칭된 Endpoint 실행
	app.MapCountriesEndpoints();
	app.MapUsersEndpoints();

	// 프레임워크 시작 로그(Microsoft.Hosting.Lifetime)는 appsettings 에서 숨기고, 주소·환경만 한 줄로 직접 출력
	app.Lifetime.ApplicationStarted.Register(() =>
	{
		if (app.Logger.IsEnabled(LogLevel.Information))
			app.Logger.LogInformation(
				"Now listening on {Addresses} ({EnvName})",
				string.Join(", ", app.Urls),
				app.Environment.EnvironmentName
			);
	});

	app.Run();
	return 0;
}
catch (Exception ex) when (ex is not HostAbortedException) // dotnet ef 가 호스트를 중단할 때 던지는 예외는 제외
{
	Log.Fatal(ex, "Application terminated unexpectedly");
	return 1; // 비정상 종료 코드 (컨테이너·오케스트레이터가 실패로 인식)
}
finally
{
	Log.CloseAndFlush(); // 버퍼에 남은 로그를 모두 쓰고 종료
}
