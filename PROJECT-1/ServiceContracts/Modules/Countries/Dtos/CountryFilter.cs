namespace ServiceContracts.Modules.Countries.Dtos;

/// <summary>
/// Country 검색 조건 (null 인 속성은 조건에서 제외)
/// </summary>
public record CountryFilter
{
	public Guid? Id { get; set; }
	public string? Name { get; set; }
}
