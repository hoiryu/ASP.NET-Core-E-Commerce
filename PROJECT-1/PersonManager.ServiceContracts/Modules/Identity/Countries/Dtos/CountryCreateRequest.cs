using System.ComponentModel.DataAnnotations;

namespace PersonManager.ServiceContracts.Modules.Identity.Countries.Dtos;

public record CountryCreateRequest
{
	[Required(ErrorMessage = "[Countries] Name can't be blank")]
	public string? Name { get; init; }
}
