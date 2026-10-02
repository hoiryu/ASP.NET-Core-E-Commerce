using ServiceContracts.Modules.Countries.Dtos;
using ServiceContracts.Modules.Users.Enums;

namespace ServiceContracts.Modules.Users.Dtos;

/// <summary>
/// User 검색 조건 (null 인 속성은 조건에서 제외, 값이 있는 조건끼리는 AND)
/// </summary>
public record UserFilter
{
	public Guid? Id { get; set; }
	public string? Name { get; set; }
	public string? Email { get; set; }
	public DateTime? DateOfBirth { get; set; }
	public GenderOptions? Gender { get; set; }
	public CountryFilter? Country { get; set; }
	public string? Address { get; set; }
	public bool? ReceiveNewsLetters { get; set; }
}
