using PersonManager.Core.Modules.Identity.Countries.Dtos;
using PersonManager.Domain.Modules.Identity.Users;

namespace PersonManager.Core.Modules.Identity.Users.Dtos;

public record UserResponse
{
	public Guid Id { get; set; }
	public string? Name { get; set; }
	public string? Email { get; set; }
	public DateTime? DateOfBirth { get; set; }
	public Gender? Gender { get; set; }
	public CountryResponse? Country { get; set; }
	public string? Address { get; set; }
	public bool ReceiveNewsLetters { get; set; }
	public double? Age { get; set; }
}
