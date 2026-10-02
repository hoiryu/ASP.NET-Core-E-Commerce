<!-- @format -->

## 시작하기

- docker 실행

```bash
docker compose up
```

## VS Code 디버깅 + Hot Reload 설정

VS Code 유저 설정(`Ctrl+Shift+P` → `Open User Settings (JSON)`)에 아래 항목 추가:

```json
"csharp.experimental.debug.hotReload": true,
"csharp.debug.hotReloadOnSave": true
```

이후 `F5`로 실행하면 디버깅과 hot reload가 동시에 작동

> `csharp.experimental.debug.hotReload`는 machine 스코프 설정이라 `.vscode/settings.json`이 아닌 유저 설정에 넣어야 합니다.

## DB 마이그레이션 (EF Core)

### 사전 준비 (최초 1회)

- EF CLI 도구 설치

```bash
dotnet tool install --global dotnet-ef
```

- `~/.dotnet/tools`를 PATH에 추가 (zsh)

```bash
echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> ~/.zprofile
source ~/.zprofile
dotnet ef --version # 버전이 출력되면 성공
```

- 연결 문자열은 `PersonManager.Api/appsettings.Development.json`의 `ConnectionStrings:DefaultConnection`에서 읽음 (gitignore 대상이라 직접 작성 필요)

### 절차

모든 명령은 `PROJECT-1` 폴더에서 실행. 마이그레이션 파일은 `Entities`에 생성되고, 설정은 `PersonManager.Api`에서 읽음

1. 엔티티/모델 수정
2. 마이그레이션 생성

```bash
dotnet ef migrations add <마이그레이션이름> --project Entities --startup-project PersonManager.Api
```

> `<마이그레이션이름>`은 생략 불가. PascalCase로 변경 내용을 설명하는 이름 사용 (파일명은 `타임스탬프_이름.cs`로 생성됨)
>
> | 상황             | 예시                   |
> | ---------------- | ---------------------- |
> | 최초 생성        | `InitialCreate`        |
> | 컬럼 추가        | `AddPhoneToUsers`      |
> | 테이블 추가      | `AddOrdersTable`       |
> | 컬럼 변경        | `ChangeUserNameLength` |
> | 시드 데이터 수정 | `FixUserCountrySeed`   |

3. DB에 적용 (테이블 생성/변경 + `HasData` 시드 데이터 INSERT)

```bash
dotnet ef database update --project Entities --startup-project PersonManager.Api
```

4. 생성된 `Entities/Migrations/` 폴더를 Git에 커밋

> 다른 PC에서 프로젝트를 받은 경우 2번은 생략하고 3번만 실행

### 마이그레이션을 새로 만들어야 하는 경우

- 엔티티 추가/삭제, 속성 추가/삭제/타입 변경
- 인덱스, 관계, 제약조건 등 설정 변경
- `HasData` 시드 데이터 변경 (`Entities/Data/Seeds/*.json` 수정 포함)

> `HasData`는 `dotnet run` 시 자동으로 INSERT되지 않음. 마이그레이션 생성 → `database update` 순서로 적용해야 DB에 반영됨

### 마이그레이션 삭제

- 아직 `database update`로 적용하지 않은 마지막 마이그레이션 삭제

```bash
dotnet ef migrations remove --project Entities --startup-project PersonManager.Api
```

- 이미 DB에 적용한 마이그레이션 삭제 (로컬 개발 DB에서만)

```bash
# 1. DB를 직전 마이그레이션 상태로 되돌림
dotnet ef database update <직전마이그레이션이름> --project Entities --startup-project PersonManager.Api

# 2. 마지막 마이그레이션 삭제
dotnet ef migrations remove --project Entities --startup-project PersonManager.Api
```

> 첫 마이그레이션까지 모두 되돌리려면 `<직전마이그레이션이름>` 자리에 `0` 입력
>
> 이미 커밋해서 다른 사람이나 서버에 적용된 마이그레이션은 삭제하지 말고, 수정 내용을 담은 새 마이그레이션을 추가

### 기타 명령

```bash
# 마이그레이션 목록과 DB 적용 여부 확인
dotnet ef migrations list --project Entities --startup-project PersonManager.Api

# 모델 변경 후 마이그레이션을 만들지 않았는지 확인
dotnet ef migrations has-pending-model-changes --project Entities --startup-project PersonManager.Api
```
