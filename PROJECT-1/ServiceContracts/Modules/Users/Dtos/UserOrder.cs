using ServiceContracts.Common.Enums;
using ServiceContracts.Modules.Countries.Dtos;

namespace ServiceContracts.Modules.Users.Dtos;

/// <summary>
/// User 정렬 조건 (null 인 속성은 정렬에서 제외)
/// 여러 개 지정하면 아래 선언 순서대로 우선순위가 적용됨
/// </summary>
public record UserOrder
{
	public OrderOptions? Id { get; set; }
	public OrderOptions? Name { get; set; }
	public OrderOptions? Email { get; set; }
	public OrderOptions? DateOfBirth { get; set; }
	public OrderOptions? Gender { get; set; }
	public CountryOrder? Country { get; set; }
	public OrderOptions? Address { get; set; }
	public OrderOptions? ReceiveNewsLetters { get; set; }
}
