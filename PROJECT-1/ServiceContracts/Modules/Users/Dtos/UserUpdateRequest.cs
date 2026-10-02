using System.ComponentModel.DataAnnotations;
using ServiceContracts.Modules.Countries.Dtos;
using ServiceContracts.Modules.Users.Enums;

namespace ServiceContracts.Modules.Users.Dtos;

public class UserUpdateRequest
{
	public Guid Id { get; set; }

	[MinLength(1, ErrorMessage = "[Users] Name can't be blank")]
	public string? Name { get; set; }

	[MinLength(1, ErrorMessage = "[Users] Email can't be blank")]
	[EmailAddress(ErrorMessage = "[Users] Email value should be a vaild email")]
	public string? Email { get; set; }
	public DateTime? DateOfBirth { get; set; }

	[EnumDataType(typeof(GenderOptions), ErrorMessage = "[Users] Gender value is invalid")]
	public GenderOptions? Gender { get; set; }
	public CountryResponse? Country { get; set; }
	public string? Address { get; set; }
	public bool? ReceiveNewsLetters { get; set; }
}
