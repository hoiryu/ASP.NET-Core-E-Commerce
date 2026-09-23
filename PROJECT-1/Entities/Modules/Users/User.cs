using Entities.Modules.Countries;

namespace Entities.Modules.Users;

/// <summary>
/// Domain Model for User
/// </summary>
public class User
{
	public Guid Id { get; set; }
	public string? Name { get; set; }
	public string? Email { get; set; }
	public DateTime? DateOfBirth { get; set; }
	public string? Gender { get; set; }
	public Country? Country { get; set; }
	public string? Address { get; set; }
	public bool ReceiveNewsLetters { get; set; }
}
