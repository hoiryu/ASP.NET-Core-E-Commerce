using ServiceContracts.Common.Enums;
using ServiceContracts.Modules.Countries.Dtos;

namespace ServiceContracts.Modules.Users.Dtos;

/// <summary>
/// User 정렬 조건 (null 인 속성은 정렬에서 제외)
/// 여러 개 지정하면 아래 선언 순서대로 우선순위가 적용됨
/// </summary>
public record UserSort
{
	public SortOrderOptions? Id { get; set; }
	public SortOrderOptions? Name { get; set; }
	public SortOrderOptions? Email { get; set; }
	public SortOrderOptions? DateOfBirth { get; set; }
	public SortOrderOptions? Gender { get; set; }
	public CountrySort? Country { get; set; }
	public SortOrderOptions? Address { get; set; }
	public SortOrderOptions? ReceiveNewsLetters { get; set; }
}
