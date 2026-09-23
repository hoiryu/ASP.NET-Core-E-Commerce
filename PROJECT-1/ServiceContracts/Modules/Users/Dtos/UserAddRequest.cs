using System.ComponentModel.DataAnnotations;
using Entities.Modules.Countries;
using Entities.Modules.Users;
using ServiceContracts.Modules.Users.Enums;

namespace ServiceContracts.Modules.Users.Dtos;

public record UserAddRequest
{
	[Required(ErrorMessage = "[Users] Name can't be blank")]
	public string? Name { get; set; }

	[Required(ErrorMessage = "[Users] Email can't be blank")]
	[EmailAddress(ErrorMessage = "[Users] Email value should be a vaild email")]
	public string? Email { get; set; }
	public DateTime? DateOfBirth { get; set; }
	public GenderOptions? Gender { get; set; }
	public Country? Country { get; set; }
	public string? Address { get; set; }
	public bool ReceiveNewsLetters { get; set; }

	public User ToEntity() =>
		new()
		{
			Name = Name,
			Email = Email,
			DateOfBirth = DateOfBirth,
			Gender = Gender?.ToString(),
			Country = Country,
			Address = Address,
			ReceiveNewsLetters = ReceiveNewsLetters,
		};
}
