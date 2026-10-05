using ServiceContracts.Common.Enums;

namespace ServiceContracts.Modules.Countries.Dtos;

/// <summary>
/// Country 정렬 조건 (null 인 속성은 정렬에서 제외)
/// 여러 개 지정하면 아래 선언 순서대로 우선순위가 적용됨
/// </summary>
public record CountryOrder
{
	public OrderOptions? Id { get; set; }
	public OrderOptions? Name { get; set; }
}
