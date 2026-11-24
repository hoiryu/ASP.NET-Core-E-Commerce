using Gridify;
using PersonManager.Entities.Modules.Identity.Users;

namespace PersonManager.Repositories.Modules.Identity.Users;

/// <summary>
/// 필터·정렬에 허용할 필드 목록 (등록되지 않은 필드는 검증에서 거부)
/// 엔티티 public 속성과 Country 의 속성(country.id, country.name)을 자동 등록하므로, 노출하면 안 되는 속성은 RemoveMap 으로 뺀다
/// </summary>
public sealed class UserGridifyMapper : GridifyMapper<User>
{
	public UserGridifyMapper()
	{
		GenerateMappings(1);
		RemoveMap("countryId"); // country.id 와 중복
	}
}
