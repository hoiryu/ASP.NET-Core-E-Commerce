---
paths:
  - "PersonManager.*/**"
  - "PersonManager.slnx"
---

# 아키텍처 (Clean Architecture)

## 계층과 참조

| 계층 | 프로젝트 | 역할 | 참조 |
|---|---|---|---|
| Presentation | `PersonManager.Presentation` | Minimal API Endpoints, Endpoint Filter, ExceptionHandler, Program.cs | Core (Infrastructure 는 DI 조립만) |
| Infrastructure | `PersonManager.Infrastructure` | AppDbContext, Configurations, Seeds, Migrations, Repository 구현, Gridify 매퍼 | Core |
| Core | `PersonManager.Core` | 유스케이스: Service, Repository 인터페이스, DTO, Mappings, 검증, 예외, 로깅 인터셉터 | Domain |
| Domain | `PersonManager.Domain` | Entity, Enum, Entity 의 도메인 계산 | 없음 |

```
Presentation ──► Core ──► Domain
     ┆            ▲
     ┆ DI 조립    │
     └┄► Infrastructure
```

- 의존은 안쪽으로만 향한다. 안쪽 계층은 바깥 계층의 타입을 쓰지 않는다.
- Presentation 은 `Program.cs` 의 `AddInfrastructure(config)` 에서만 Infrastructure 를 쓴다.
- Repository 인터페이스는 **Core** 에 둔다 (Use Case 가 정의, Infrastructure 가 구현).

## 계층별 규칙

| 계층 | 규칙 |
|---|---|
| Domain | `PackageReference`·`ProjectReference` 없음. Entity 는 POCO (attribute·`virtual` 내비게이션·EF 타입 금지, 매핑은 Fluent API 로만) |
| Core | EF·Gridify·Serilog 패키지 금지. 로깅은 `ILogger`, 현재 시각은 `TimeProvider` 주입. 입력 검증은 `ValidationHelper` 로 스스로 한다 |
| Infrastructure | EF·Gridify 는 여기서만. `ListQuery` → `GridifyQuery`, `Paging<T>` → `PagedResult<T>` 변환. 쓰기(`SaveChangesAsync`·`ExecuteUpdateAsync`·`ExecuteDeleteAsync`)는 `.TranslateDbExceptions()` 로 감싸 Core 예외로 변환 |
| Presentation | Core 타입만 사용. `ApiExceptionHandler` 는 Core 예외(`InvalidRequestException` → 400, `ConflictException` → 409)와 바인딩 오류(`BadHttpRequestException`)만 처리 |

## 폴더 구조

```
PersonManager.Domain/
  Modules/Identity/{Users,Countries}/        Entity, Enum
PersonManager.Core/
  Common/Exceptions/                         InvalidRequestException, ConflictException
  Common/Helpers/                            ValidationHelper
  Common/Logging/                            [LogExecutionTime], 인터셉터, AddLoggedScoped
  Common/Queries/                            ListQuery, PagedResult<T>
  Modules/Identity/{Users,Countries}/        I*Service, *Service, I*Repository, *Mappings, Dtos/
  DependencyInjection.cs                     AddCore()
PersonManager.Infrastructure/
  Data/                                      AppDbContext, Configurations/, Seeds/, Migrations/
  Common/Extensions/                         GridifyExtensions, DbExceptionExtensions
  Modules/Identity/{Users,Countries}/        *Repository, *GridifyMapper
  DependencyInjection.cs                     AddInfrastructure(IConfiguration)
PersonManager.Presentation/
  Common/ExceptionHandlers/                  ApiExceptionHandler
  Endpoints/Common/                          Dtos/, EndpointFilters/, Parameters/ListQueryParameters
  Endpoints/Modules/Identity/{Users,Countries}/  *Endpoints
  Program.cs
```

- namespace 는 `{프로젝트명}.{폴더 경로}` (예: `PersonManager.Core.Modules.Identity.Users.Dtos`).

## 모듈 추가 순서

1. Domain: `Modules/{그룹}/{모듈}/` 에 Entity
2. Core: `I*Repository`, `I*Service`, `*Service`(`[LogExecutionTime]`), `*Mappings`, `Dtos/` → `AddCore()` 에 `AddLoggedScoped` 등록
3. Infrastructure: `*Configuration`, `AppDbContext` 의 `DbSet`, `*Repository`, `*GridifyMapper` → `AddInfrastructure()` 에 등록
4. Presentation: `*Endpoints` → `Program.cs` 에서 `Map*Endpoints()`
5. 마이그레이션: `dotnet ef migrations add <이름> --project PersonManager.Infrastructure --startup-project PersonManager.Presentation`

## 알려진 타협

- `ListQuery.Filter` 와 bulk 수정·삭제의 filter 는 Gridify 문법 문자열이다 (Core 는 패키지는 모르지만 문법은 따른다).
- Gridify 매퍼의 `GenerateMappings` 때문에 Entity 속성 이름이 곧 API 필터 필드 이름이다.
- 이메일·국가 이름 중복은 DB 유니크 인덱스, Country 삭제 시 User 의 country null 처리는 DB FK(`SetNull`)가 맡는다.
