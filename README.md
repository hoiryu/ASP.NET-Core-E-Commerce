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
