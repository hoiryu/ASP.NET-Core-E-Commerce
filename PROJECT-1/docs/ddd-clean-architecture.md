# DDD + Clean Architecture 전환 계획

> 상태: **계획만 있음. 아직 진행하지 않음** (2026-10-07 작성)
> 나중에 리팩터링할 때 참고하려고 남겨 두는 문서입니다. 아래 작업 중 지금 적용된 것은 없습니다.

---

## 1. 배경: 지금 구조의 문제

현재 솔루션 `PersonManager.slnx`는 프로젝트 4개로 된 전형적인 N-Layer 구조입니다.

```
PersonManager.Api → Services → ServiceContracts → Entities
```

| 문제 | 위치 |
|---|---|
| 도메인 프로젝트(`Entities`)가 EF Core 패키지와 `AppDbContext`, 마이그레이션, Seed를 함께 가지고 있음. 도메인이 인프라에 의존함 | `Entities/Entities.csproj`, `Entities/Data/` |
| 엔티티가 빈약함(anemic). public setter만 있어서 아무 값이나 넣을 수 있음 | `Entities/Modules/Users/User.cs`, `Entities/Modules/Countries/Country.cs` |
| 규칙이 서비스와 DTO에 흩어져 있음. 이메일 형식은 DTO의 `[EmailAddress]`에서, 국가 존재 여부와 이름 중복은 서비스에서 검사함 | `Services/Modules/*/…Service.cs` |
| DTO가 엔티티를 직접 만들어서 `ServiceContracts`가 `Entities`를 참조함 | `UserCreateRequest.ToEntity()`, `UserResponse`의 `ToResponse()` |
| `Gender`가 엔티티에서는 `string`이고 DTO에서는 `GenderOptions` enum이라 변환이 곳곳에 있음 | `UserQueryExtensions`, `UserExtensions` |

---

## 2. DDD 패턴 개요

### 2-1. 구조 패턴: 프로젝트를 어떻게 나눌지

| 패턴 | 핵심 | 복잡도 |
|---|---|---|
| **Layered (전통 DDD 4계층)** | UI → Application → Domain → Infrastructure. Domain이 Infrastructure를 참조해도 허용하는 느슨한 형태 | 낮음 |
| **Clean / Onion** | 의존성이 안쪽(Domain)으로만 향함. Infrastructure가 안쪽 계층의 인터페이스를 구현 | 중간 |
| **Hexagonal (Ports & Adapters)** | Onion과 사상은 같고, 외부 연결을 Port(인터페이스)와 Adapter(구현)라는 용어로 설명 | 중간 |
| **Vertical Slice** | 계층 대신 기능 단위로 나눔(`Users/CreateUser.cs` 하나에 요청·처리·응답) | 낮음~중간 |
| **CQRS** | 쓰기(Command)와 조회(Query) 경로를 분리 | 높음 |

### 2-2. 전술 패턴: 도메인 코드 안에서

| 패턴 | 설명 | 이 프로젝트 예 |
|---|---|---|
| **Entity** | Id로 구분되는 객체 | `User`, `Country` |
| **Value Object** | Id 없이 값으로 비교하는 불변 객체 | `Email` |
| **Aggregate / Root** | 함께 일관성을 지켜야 하는 묶음. 외부에서는 Root를 통해서만 수정하고, 다른 Aggregate는 Id로 참조 | `User` aggregate, `Country` aggregate |
| **Repository** | Aggregate 하나를 저장하고 불러오는 추상화 | `IUserRepository` |
| **Factory** | 생성 규칙을 담는 곳 | `User.Create(...)` |
| **Domain Service** | 엔티티 하나에 넣기 애매한 규칙 | 이메일 중복 검사 |
| **Domain Event** | "무슨 일이 일어났다"는 사실을 알리는 메시지 | `UserCreated` |
| **Specification** | 조회 조건을 객체로 표현해서 Repository에 넘김 | (ardalis.Specification 라이브러리) |

---

## 3. Value Object (값 객체)

**Id가 없고, 값이 같으면 같은 것으로 보는 작은 불변 객체**입니다.

- 비유: 사람(`User`)은 이름이 같아도 **다른 사람**이므로 Id로 구분합니다. 이것이 Entity입니다.
  1000원짜리 지폐는 어떤 지폐든 **같은 1000원**이므로 값만 중요합니다. 이것이 Value Object입니다.

### 지금 코드의 문제

```csharp
public class User
{
	public string? Email { get; set; } // 아무 문자열이나 들어감
}

user.Email = "abc"; // 이메일 형식이 아닌데도 들어감
user.Email = "";    // 빈 값도 들어감
```

이메일 검사는 DTO의 `[EmailAddress]`에만 있습니다. 그래서 다른 경로로 `User`를 만들면 검사를 거치지 않습니다.

### Value Object로 바꾸면

```csharp
public record Email
{
	public string Value { get; }

	private Email(string value) => Value = value;

	public static Email Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
			throw new DomainException("이메일 형식이 아닙니다");

		return new Email(value.Trim().ToLower());
	}
}

public class User
{
	public Email Email { get; private set; } // string 대신 Email 타입
}
```

- `Email.Create()`를 거쳐야만 만들 수 있으므로 **`Email` 타입이면 항상 올바른 이메일**입니다.
- `record`라서 값으로 비교합니다: `Email.Create("a@b.com") == Email.Create("a@b.com")`의 결과는 `true`
- 불변이라서 바꾸려면 새로 만들어야 합니다.
- EF Core에서는 `builder.Property(u => u.Email).HasConversion(e => e.Value, v => Email.Create(v))`로 저장합니다. 컬럼은 그대로 `nvarchar`입니다.

흔한 예로 `Email`, `Money(금액 + 통화)`, `Address(시/구/도로명)`, `DateRange(시작 ~ 끝)`가 있습니다.

---

## 4. Domain Event (도메인 이벤트)

**도메인에서 의미 있는 일이 일어났다는 사실을 알리는 메시지**입니다. 이름은 보통 과거형으로 짓습니다(`UserCreated`, `CountryDeleted`).

- 비유: 쇼핑몰에서 "주문 완료"가 일어나면 재고 차감, 확인 메일, 포인트 적립을 해야 합니다.
  주문 코드가 이걸 전부 직접 호출하면 주문 로직이 메일과 포인트까지 알아야 합니다.
  대신 "주문 완료됨" 이벤트를 **한 번 발행**하고, 각 담당자가 듣고 알아서 처리하게 합니다.

### 예: "뉴스레터 수신 동의자에게 가입 환영 메일 발송"

**이벤트 없이:**

```csharp
public async Task<UserResponse> CreateUser(...)
{
	var user = User.Create(...);
	_db.Users.Add(user);
	await _db.SaveChangesAsync();

	if (user.ReceiveNewsLetters)
		await _emailSender.SendWelcome(user.Email); // 서비스가 메일까지 알아야 함
	// "가입 로그", "슬랙 알림"이 추가되면 여기가 계속 늘어남
}
```

**이벤트 사용:**

```csharp
// Domain: User 는 "생성됐다"는 사실만 기록
public static User Create(...)
{
	var user = new User(...);
	user.AddDomainEvent(new UserCreated(user.Id, user.Email, user.ReceiveNewsLetters));
	return user;
}

// 별도 핸들러: 이벤트를 듣고 처리
public class SendWelcomeMailHandler : IDomainEventHandler<UserCreated>
{
	public Task Handle(UserCreated e) =>
		e.ReceiveNewsLetters ? _emailSender.SendWelcome(e.Email) : Task.CompletedTask;
}
```

- `UsersService`는 메일 기능의 존재조차 모릅니다.
- 기능이 늘어나면 핸들러만 추가하면 됩니다.
- **단점:** `SaveChanges` 직후 모인 이벤트를 꺼내 핸들러에 전달하는 **디스패처**가 필요합니다. 직접 구현하거나 MediatR 같은 라이브러리를 써야 하고, 구조가 복잡해지는 주된 원인입니다.

---

## 5. 이 프로젝트에 권장하는 방향 (단순안)

### 결정 요약

| 항목 | 결정 |
|---|---|
| 구조 | Clean Architecture 프로젝트 4개 |
| 데이터 접근 | Application에 `IApplicationDbContext` 인터페이스를 둠(Jason Taylor 템플릿 방식) |
| 조회 | 기존 Filter / Order / Paging `IQueryable` 확장을 그대로 재사용 |
| 도메인 모델 | `User.Create/Update`, `Country.Create/Rename`, private setter |
| `Gender` | Domain enum 하나로 통합(`GenderOptions` 삭제) |
| Value Object (`Email`) | 선택(넣는다면 `Email` 하나만) |
| Domain Event | 보류(필요한 요구사항이 생기면 도입) |
| `User.Country` 내비게이션 | 유지(Country 이름 필터·정렬 쿼리를 그대로 쓰기 위해) |

### 목표 구조

```
PROJECT-1/
├─ PersonManager.slnx
├─ Directory.Build.props                 # net10.0, Nullable, ImplicitUsings 공통화
└─ src/
   ├─ PersonManager.Domain/              # PackageReference 없음
   │  ├─ Common/     DomainException.cs
   │  ├─ Countries/  Country.cs
   │  └─ Users/      User.cs, Gender.cs
   ├─ PersonManager.Application/         # → Domain
   │  ├─ Common/     IApplicationDbContext.cs, Dtos/Paging.cs, Enums/OrderOptions.cs,
   │  │              Extensions/QueryableExtensions.cs, Helpers/ValidationHelper.cs,
   │  │              DependencyInjection.cs
   │  ├─ Countries/  ICountriesService.cs, CountriesService.cs, Dtos/, Extensions/, Mappings.cs
   │  └─ Users/      IUsersService.cs, UsersService.cs, Dtos/, Extensions/, Mappings.cs
   ├─ PersonManager.Infrastructure/      # → Application (EF Core 패키지는 여기만)
   │  ├─ Persistence/ AppDbContext.cs, Configurations/, Seeds/, Migrations/
   │  └─ DependencyInjection.cs           # AddInfrastructure(IConfiguration)
   └─ PersonManager.Api/                 # → Application, Infrastructure
      ├─ Controllers/Modules/{Users,Countries}/
      ├─ Middleware/ExceptionHandlingMiddleware.cs
      └─ Program.cs
```

### `IApplicationDbContext` 예시

```csharp
// Application/Common/IApplicationDbContext.cs
public interface IApplicationDbContext
{
	DbSet<User> Users { get; }
	DbSet<Country> Countries { get; }
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

// Infrastructure/Persistence/AppDbContext.cs
public class AppDbContext(DbContextOptions<AppDbContext> options)
	: DbContext(options), IApplicationDbContext { ... }

// Application 서비스는 AppDbContext 대신 인터페이스를 주입받음
public class UsersService(IApplicationDbContext _db) : IUsersService { ... }
```

> `DbSet<T>`를 쓰려면 Application이 `Microsoft.EntityFrameworkCore` 패키지 하나를 참조해야 합니다(SqlServer 패키지는 아님).
> 순수하게 가려면 Application도 EF를 몰라야 하지만, 실무 템플릿에서도 흔히 받아들이는 타협입니다.

### 엔티티 예시

```csharp
public class User
{
	public Guid Id { get; private set; }
	public string Name { get; private set; } = default!;
	public string Email { get; private set; } = default!;
	public Gender? Gender { get; private set; }
	public Guid? CountryId { get; private set; }
	public Country? Country { get; private set; }
	// ...

	private User() { } // EF Core 용

	public static User Create(string name, string email, Gender? gender, Guid? countryId, ...)
	{
		if (string.IsNullOrWhiteSpace(name))
			throw new DomainException("[Users] Name can't be blank");

		return new User { Id = Guid.NewGuid(), Name = name, Email = email, Gender = gender, CountryId = countryId, ... };
	}

	public void Update(string? name, string? email, ...)
	{
		// null 은 "보내지 않음"으로 보고 기존 값을 유지(현재 UpdateUser 동작과 동일)
		Name = name ?? Name;
		Email = email ?? Email;
		// ...
	}
}
```

### 보류한 대안 (복잡도 때문에)

- **Repository를 Domain에 두는 방식**(eShop, ardalis): Aggregate마다 `IUserRepository`를 둡니다. 그런데 Filter/Order/Paging DTO는 Application에 있어서 목록 조회를 따로 처리해야 합니다.
- **CQRS-lite**: 쓰기는 Repository가, 조회는 `IUserQueries`(Application 인터페이스, Infrastructure 구현)가 맡습니다. 깔끔하지만 파일과 인터페이스가 많아집니다.
- **Aggregate 간 Id 참조만 허용**: `User.Country` 내비게이션을 없애고 `CountryId`만 남기는 방식입니다. Country 이름 필터·정렬에 JOIN을 직접 써야 합니다.
- **Country 삭제 시 `CountryId` null 처리**: 지금처럼 서비스에서 트랜잭션 + `ExecuteUpdate`를 쓰는 대신, FK에 `OnDelete(DeleteBehavior.SetNull)`을 거는 방식입니다(마이그레이션 추가 필요).

---

## 6. 현재 → 목표 파일 매핑

| 현재 | 목표 |
|---|---|
| `Entities/Modules/Users/User.cs`, `Entities/Modules/Countries/Country.cs` | `src/PersonManager.Domain/{Users,Countries}/` (팩토리 + private setter) |
| `ServiceContracts/Modules/Users/Enums/GenderOptions.cs` | `src/PersonManager.Domain/Users/Gender.cs` |
| `Entities/Data/AppDbContext.cs`, `Data/Configurations/`, `Data/Seeds/` | `src/PersonManager.Infrastructure/Persistence/` |
| `Entities/Migrations/` | `src/PersonManager.Infrastructure/Persistence/Migrations/` (namespace만 수정) |
| `ServiceContracts/Common/*` | `src/PersonManager.Application/Common/*` |
| `ServiceContracts/Modules/*` (인터페이스, DTO) | `src/PersonManager.Application/{Users,Countries}/` |
| `Services/Modules/*/…Service.cs` | `src/PersonManager.Application/{Users,Countries}/` |
| `Services/Modules/*/Extensions/*QueryExtensions.cs` | `src/PersonManager.Application/{Users,Countries}/Extensions/` |
| `Services/Common/Extensions/QueryableExtensions.cs` | `src/PersonManager.Application/Common/Extensions/` |
| `Services/Common/Helpers/ValidationHelper.cs` | `src/PersonManager.Application/Common/Helpers/` |
| DTO 안의 `ToEntity()` / `ToResponse()` | `src/PersonManager.Application/{Users,Countries}/Mappings.cs` |
| `Program.cs`의 수동 DI + `AddDbContext` | `AddApplication()` / `AddInfrastructure(config)` |

---

## 7. 작업 단계

1. `src/`를 만들고 `git mv`로 파일을 옮겨 이력을 보존합니다. 새 csproj 4개를 만들고 `ProjectReference`와 `PersonManager.slnx`를 갱신하며 `Directory.Build.props`를 추가합니다.
2. **Domain**: `DomainException`, `Gender` enum, `User`/`Country`의 팩토리·행위 메서드와 private setter를 작성합니다.
3. **Application**: DTO, 서비스, 인터페이스를 옮기고 `IApplicationDbContext`를 정의합니다. 서비스가 이 인터페이스를 주입받게 하고, `ToEntity()` 대신 `User.Create(...)`, 수정 시 `user.Update(...)`를 쓰게 합니다.
4. **Infrastructure**: `AppDbContext`(`IApplicationDbContext` 구현), Configurations, Seeds, Migrations를 옮기고 namespace를 수정합니다. `AddInfrastructure()`를 작성합니다.
   - 마이그레이션은 **새로 만들지 않습니다**(이미 DB에 적용됨). 마이그레이션 Id가 그대로면 `__EFMigrationsHistory`와 맞습니다.
5. **Api**: `Program.cs`를 정리하고 컨트롤러 using을 수정하고 예외 미들웨어를 추가합니다(`DomainException`/`ArgumentException`은 400).
6. 기존 `Entities/`, `ServiceContracts/`, `Services/` 폴더를 삭제합니다.
7. 경로를 수정합니다.
   - `.vscode/tasks.json`: `--project Entities --startup-project PersonManager.Api` → `--project src/PersonManager.Infrastructure --startup-project src/PersonManager.Api`
   - 상위 `README.md` EF 명령어 표의 설명
   - `PersonManager.Infrastructure.csproj`에 `Seeds/*.json`의 `CopyToOutputDirectory`를 옮김
8. (선택) 서비스와 컨트롤러를 `async`로 바꿉니다.

---

## 8. 주의사항

- **Seed와 private setter**: `AppDbContext.OnModelCreating`은 지금 `JsonSerializer.Deserialize<List<User>>`로 엔티티를 직접 만듭니다.
  setter가 private이 되면 역직렬화가 안 되므로 `Seeds/` 안에 `UserSeed`/`CountrySeed` record를 두고, `HasData`에 **익명 객체**로 넘깁니다.
  값을 기존과 똑같이 둬야 마이그레이션 차이가 생기지 않습니다.
- **`Gender` enum 변환**: `UserConfiguration`에서 `builder.Property(u => u.Gender).HasConversion<string>().HasMaxLength(10)`을 써야 기존 `nvarchar(10)` 컬럼과 Seed 값(`"Male"`, `"Female"`)이 그대로 유지됩니다.
- **옮긴 직후 모델 변경 확인**: 파일을 옮기고 엔티티를 바꾼 다음 아래 명령이 **변경 없음**을 보고해야 합니다. 변경이 있다고 나오면 매핑이 달라진 것입니다.
  ```bash
  dotnet ef migrations has-pending-model-changes --project src/PersonManager.Infrastructure --startup-project src/PersonManager.Api
  ```
- **API 계약 유지**: URL, 쿼리 파라미터(`filter.*`, `order.*`, `paging.*`), JSON 응답 모양은 바꾸지 않습니다. `Gender`는 `JsonStringEnumConverter` 덕분에 문자열 그대로 나갑니다.

---

## 9. 검증 체크리스트

- [ ] `dotnet build PersonManager.slnx`에서 경고와 에러가 없음
- [ ] `src/PersonManager.Domain/*.csproj`에 `PackageReference`와 `ProjectReference`가 없음
- [ ] `has-pending-model-changes`가 변경 없음을 보고함
- [ ] `docker compose up -d` 후 `dotnet ef database update`가 정상 실행됨
- [ ] `dotnet run --project src/PersonManager.Api` 후 다음 동작이 기존과 같음
  - [ ] `GET /api/users?filter.country.name=…&order.name=ASC&paging.take=5`
  - [ ] `GET /api/countries`
  - [ ] `POST /api/users`에 잘못된 값을 보내면 400
  - [ ] 없는 Id로 `PATCH` / `DELETE`하면 404
  - [ ] Country를 `DELETE`하면 해당 User들의 `country`가 null이 됨
