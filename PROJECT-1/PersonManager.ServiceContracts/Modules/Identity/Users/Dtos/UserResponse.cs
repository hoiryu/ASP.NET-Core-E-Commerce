using PersonManager.ServiceContracts.Modules.Identity.Countries.Dtos;
using PersonManager.ServiceContracts.Modules.Identity.Users.Enums;

namespace PersonManager.ServiceContracts.Modules.Identity.Users.Dtos;

public record UserResponse
{
	public Guid Id { get; set; }
	public string? Name { get; set; }
	public string? Email { get; set; }
	public DateTime? DateOfBirth { get; set; }
	public GenderOptions? Gender { get; set; }
	public CountryResponse? Country { get; set; }
	public string? Address { get; set; }
	public bool ReceiveNewsLetters { get; set; }
	public double? Age { get; set; }

	public override string ToString()
	{
		return $"User Id: {Id}, Name: {Name}, Email: {Email}, DateOfBirth: {DateOfBirth}, Gender: {Gender}, Country: {Country}";
	}
}
