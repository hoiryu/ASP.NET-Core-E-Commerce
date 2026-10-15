namespace PersonManager.ServiceContracts.Modules.Identity.Countries.Dtos;

public record CountryCreateRequest
{
	public string? Name { get; init; }
}
