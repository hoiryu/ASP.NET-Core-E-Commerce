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

## EF Core 명령어

각 프로젝트 폴더(`PROJECT-*`)를 VS Code로 열고 `Ctrl+Shift+P` → `Tasks: Run Task`에서 실행 (각 프로젝트의 `.vscode/tasks.json`에 정의)

| Task                              | 명령어                                                                                                  |
| --------------------------------- | ------------------------------------------------------------------------------------------------------- |
| EF: Add Migration                 | `dotnet ef migrations add <이름> --project <Entities> --startup-project <Api>`                |
| EF: Update Database               | `dotnet ef database update --project <Entities> --startup-project <Api>`                      |
| EF: Update Database To Migration  | `dotnet ef database update <대상> --project <Entities> --startup-project <Api>`               |
| EF: Remove Last Migration         | `dotnet ef migrations remove --project <Entities> --startup-project <Api>`                    |
| EF: List Migrations               | `dotnet ef migrations list --project <Entities> --startup-project <Api>`                      |
| EF: Check Pending Model Changes   | `dotnet ef migrations has-pending-model-changes --project <Entities> --startup-project <Api>` |

> `<Entities>`: DbContext·마이그레이션이 있는 프로젝트, `<Api>`: 설정(연결 문자열)을 읽는 시작 프로젝트
