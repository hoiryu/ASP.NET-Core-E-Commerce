using Entities.Users;
using ServiceContracts.Users.Enums;

namespace ServiceContracts.Users.Dtos;

public record UserAddRequest
{
	public string? Name { get; set; }
	public string? Email { get; set; }
	public DateTime? DateOfBirth { get; set; }
	public GenderOptions? Gender { get; set; }
	public Guid? CountryId { get; set; }
	public string? Address { get; set; }
	public bool ReceiveNewsLetters { get; set; }

	public User ToEntity() =>
		new()
		{
			Name = Name,
			Email = Email,
			DateOfBirth = DateOfBirth,
			Gender = Gender.ToString(),
			CountryId = CountryId,
			Address = Address,
			ReceiveNewsLetters = ReceiveNewsLetters,
		};
}
