---
name: git-commit
description: Stage and commit changes using the Conventional Commits format with English messages. Use when the user asks to commit, make a commit, or "git-commit". Groups unrelated changes into separate commits.
---

# git-commit

Create clean commits from the current working tree using **Conventional Commits**, written in **English**.

## Workflow

1. **Inspect the working tree** — run in parallel:
   - `git status`
   - `git diff` (unstaged) and `git diff --staged` (staged)
   - `git log --oneline -10` to match the repo's existing tone
   - `git branch --show-current`

2. **Decide commit boundaries.** If the changes cover multiple unrelated concerns, split them into
   separate commits (stage with `git add <path>` or `git add -p`). One commit = one logical change.

3. **If on the default branch** (`main`/`master`) and the change is non-trivial, create a topic
   branch first unless the user said to commit directly.

4. **Write the message** (see format below), then commit with a HEREDOC so the body formats correctly:

   ```
   git commit -m "$(cat <<'EOF'
   feat(binding): add custom IModelBinder for comma-separated ids

   Bind repeated query values into a List<int> so controllers can accept
   /cities?ids=1,2,3 without manual parsing.
   EOF
   )"
   ```

5. **Verify** with `git status` and `git log --oneline -3`. Report what was committed. Do **not**
   push unless the user asks.

## Conventional Commits format

```
<type>(<optional scope>): <description>

<optional body — what changed and why, wrapped at ~72 chars>

<optional footer — BREAKING CHANGE:, refs #123>
```

### Rules for the subject line
- `type` is lowercase, from the list below.
- `scope` is optional, lowercase, a noun for the affected area (e.g. `api`, `services`, `routing`, `di`, `validation`, `config`). Omit if it spans many areas.
- `description` is imperative mood, lowercase start, **no trailing period**, ≤ 72 chars
  ("add X", not "added X" / "adds X").
- Subject must be able to complete the sentence: *"If applied, this commit will …"*.

### Types
| type | use for |
|---|---|
| `feat` | a new feature or capability |
| `fix` | a bug fix |
| `refactor` | code change that neither fixes a bug nor adds a feature |
| `perf` | performance improvement |
| `docs` | documentation only (README, comments, XML docs) |
| `test` | adding or fixing tests |
| `build` | build system, `.csproj`, `.slnx`, NuGet dependencies |
| `ci` | CI/CD pipeline config |
| `style` | formatting, whitespace, no code-behavior change |
| `chore` | tooling, `.gitignore`, housekeeping with no src impact |
| `revert` | reverts a previous commit |

### Breaking changes
Add `!` after type/scope **and** a footer:

```
feat(api)!: return 404 instead of empty list for unknown city

BREAKING CHANGE: GET /cities/{id} now responds 404 when the id is not found.
```

## Body guidance
- Add a body when the *why* isn't obvious from the subject; skip it for trivial changes.
- Explain intent and consequences, not a line-by-line diff recap.
- Use `-` bullets for multiple distinct points.

## Do not
- Do not add `Co-Authored-By` or other trailers unless the user asks.
- Do not run `git add -A` blindly — stage deliberately so unrelated files don't ride along.
- Do not amend or force-push published commits without explicit instruction.
- Do not commit secrets, `appsettings.*.json` with real credentials, or `bin/`/`obj/`.

## Examples

```
feat(config): bind Api section to strongly-typed ApiOptions
fix(routing): allow optional trailing slash on city detail route
refactor(services): move seed data into CitiesDataProvider
build(deps): upgrade Microsoft.AspNetCore.OpenApi to 8.0.8
docs(readme): document the ServiceContracts project layout
test(cities): cover GetCityById with an unknown id
chore: add .vs/ and *.user to .gitignore
```
