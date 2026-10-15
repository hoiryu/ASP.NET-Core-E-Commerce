# API 목록 조회 쿼리스트링 (Gridify)

`GET /api/users`, `GET /api/countries` 는 Gridify 의 `GridifyQuery` 로 필터·정렬·페이징을 받는다.

```
?filter=<조건>&orderBy=<정렬>&page=<페이지>&pageSize=<개수>
```

| 파라미터 | 설명 | 기본값 |
|---|---|---|
| `filter` | 검색 조건 | 없음 (전체) |
| `orderBy` | 정렬 | 없음 (DB 순서) |
| `page` | 페이지 번호, 1부터 시작 | 1 |
| `pageSize` | 한 페이지 건수 | 20 |

응답은 `{ "count": <필터 결과 전체 건수>, "data": [ <현재 페이지> ] }` 이다.

## 필드

필드 이름은 대소문자를 구분하지 않는다. 허용 필드는 각 Repository 의 `_mapper` 에 등록되어 있다.

| API | 필드 | 정의 위치 |
|---|---|---|
| `/api/countries` | `id`, `name` | `PersonManager.Repositories/Modules/Countries/CountriesRepository.cs` |
| `/api/users` | `id`, `name`, `email`, `dateOfBirth`, `gender`, `address`, `receiveNewsLetters`, `country.id`, `country.name` | `PersonManager.Repositories/Modules/Users/UsersRepository.cs` |

등록되지 않은 필드를 쓰면 `ArgumentException: Field 'xxx' is not mapped` 가 난다.
필드를 추가하거나 바꾸면 mapper 와 이 문서를 함께 수정한다.

## 연산자

| 연산자 | 의미 | 예시 |
|---|---|---|
| `=` | 같음 | `name=Korea` |
| `!=` | 다름 | `name!=Korea` |
| `=*` | 포함 | `name=*Kor` |
| `!*` | 포함하지 않음 | `name!*Kor` |
| `^` | 시작함 | `name^Th` |
| `!^` | 시작하지 않음 | `name!^Th` |
| `$` | 끝남 | `name$land` |
| `!$` | 끝나지 않음 | `name!$land` |
| `>` `<` `>=` `<=` | 크기 비교 (날짜, 숫자) | `dateOfBirth>=1990-01-01` |
| `/i` (값 뒤) | 대소문자 무시 | `name=korea/i` |

현재 DB collation 은 대소문자를 구분하지 않으므로 `/i` 는 쓰지 않는다(`name=korea` 도 `Korea` 와 일치).

## 조건 조합

| 기호 | 의미 | 예시 |
|---|---|---|
| `,` | AND | `gender=Male,country.name=Korea` |
| `\|` | OR | `name=Korea\|name=Japan` |
| `( )` | 묶기 | `(name=*Kim\|name=*Lee),gender=Female` |

## 값 형식

| 타입 | 형식 | 예시 |
|---|---|---|
| 문자열 | 따옴표 없이 | `name=South Korea` |
| Guid | 그대로 | `id=00000000-0000-0000-0000-000000000001` |
| 날짜 | ISO 8601 | `dateOfBirth>=2000-01-01` |
| bool | `true` / `false` | `receiveNewsLetters=true` |
| gender | DB 문자열 그대로 | `gender=Male` / `Female` / `Other` |
| null | `null` 키워드 | `address=null`, `address!=null` |

날짜는 시간까지 정확히 일치해야 한다. 하루 전체를 찾으려면 범위로 쓴다:
`filter=dateOfBirth>=2000-01-01,dateOfBirth<2000-01-02`

## 정렬

```
orderBy=name                        오름차순 (기본)
orderBy=name desc                   내림차순
orderBy=country.name, name desc     여러 개는 쉼표로, 앞쪽이 우선
```

## 특수문자 이스케이프

값 안의 `, | ( ) \ /i` 앞에는 `\` 를 붙인다. 예: `filter=name=Smoke\,\(Land\)` → 값 `Smoke,(Land)`.

## URL 인코딩

`filter` 와 `orderBy` 값은 반드시 URL 인코딩해서 보낸다. 공백, `|`, `^`, `\`, `+`, `&` 는 인코딩하지 않으면 깨질 수 있다.

- JS: `new URLSearchParams({ filter: "name=*Kim|name=*Lee", orderBy: "name desc" })`
- curl: `curl -G URL --data-urlencode "filter=..." --data-urlencode "orderBy=..."`

## 예시

```
# 이름에 "Kim" 이 들어간 한국 사용자, 이름순, 두 번째 페이지 10건
GET /api/users?filter=name=*Kim,country.name=Korea&orderBy=name&page=2&pageSize=10

# 뉴스레터 구독 중이고 주소가 있는 여성 사용자
GET /api/users?filter=gender=Female,receiveNewsLetters=true,address!=null

# 1990년대생, 생일 빠른 순
GET /api/users?filter=dateOfBirth>=1990-01-01,dateOfBirth<2000-01-01&orderBy=dateOfBirth

# 이메일이 gmail 또는 naver
GET /api/users?filter=email$@gmail.com|email$@naver.com

# 국가 이름이 정확히 Korea
GET /api/countries?filter=name=Korea

# T 로 시작하는 국가, 이름 역순
GET /api/countries?filter=name^T&orderBy=name desc
```

## 수정·삭제

수정·삭제는 `filter` 로 대상을 고르는 엔드포인트 하나뿐이다. 한 건만 바꾸려면 `filter=id=<guid>` 를 쓴다. `orderBy`·`page`·`pageSize` 는 무시하고, 응답은 `{ "count": <영향받은 행 수> }` 이다.

```
PATCH  /api/users?filter=<조건>       body: UserUpdateRequest (null 필드는 변경 안 함)
DELETE /api/users?filter=<조건>
PATCH  /api/countries?filter=<조건>   body: CountryUpdateRequest
DELETE /api/countries?filter=<조건>   (참조하는 User 의 country 는 null 로 변경)
```

- `filter` 가 비면 400 (전체 행 수정·삭제 방지)
- `email` 을 바꿀 때 대상이 여러 행이면 유니크 인덱스 위반으로 UPDATE 전체가 실패한다
- `ExecuteUpdateAsync`/`ExecuteDeleteAsync` 로 SQL 한 번에 실행하며 `SaveChanges` 를 거치지 않는다

```
# 한 건 수정
PATCH /api/users?filter=id=00000000-0000-0000-0000-000000000001   {"name": "Kim"}

# 한국 사용자 전원 뉴스레터 구독
PATCH /api/users?filter=country.name=Korea   {"receiveNewsLetters": true}

# 주소가 없는 사용자 삭제
DELETE /api/users?filter=address=null

# T 로 시작하는 국가 삭제
DELETE /api/countries?filter=name^T
```
