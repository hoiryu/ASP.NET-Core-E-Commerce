using PersonManager.Domain.Modules.Identity.Countries;

namespace PersonManager.Domain.Modules.Identity.Users;

public class User
{
	public Guid Id { get; set; }

	public string? Name { get; set; }

	public string? Email { get; set; }

	public DateTime? DateOfBirth { get; set; }

	public Gender? Gender { get; set; }

	public string? Address { get; set; }

	public bool ReceiveNewsLetters { get; set; }

	public Guid? CountryId { get; set; }

	public Country? Country { get; set; }

	/// <summary>
	/// now 기준 나이 (년, 소수 포함). 생년월일이 없으면 null
	/// </summary>
	public double? GetAge(DateTime now) =>
		DateOfBirth is DateTime dateOfBirth ? (now - dateOfBirth).TotalDays / 365.25 : null;
}
