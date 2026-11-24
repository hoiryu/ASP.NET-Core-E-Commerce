using Gridify;
using PersonManager.Entities.Modules.Identity.Countries;

namespace PersonManager.Repositories.Modules.Identity.Countries;

/// <summary>
/// 필터·정렬에 허용할 필드 목록 (등록되지 않은 필드는 검증에서 거부)
/// 엔티티 public 속성을 자동 등록하므로, 노출하면 안 되는 속성은 RemoveMap 으로 뺀다
/// </summary>
public sealed class CountryGridifyMapper : GridifyMapper<Country>
{
	public CountryGridifyMapper()
	{
		GenerateMappings();
	}
}
