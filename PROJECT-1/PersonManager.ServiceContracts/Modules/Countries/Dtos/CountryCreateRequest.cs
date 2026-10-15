namespace PersonManager.ServiceContracts.Modules.Countries.Dtos;

public record CountryCreateRequest
{
	public string? Name { get; init; }
}
