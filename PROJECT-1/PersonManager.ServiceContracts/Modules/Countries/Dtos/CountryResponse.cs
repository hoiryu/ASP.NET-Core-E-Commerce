namespace PersonManager.ServiceContracts.Modules.Countries.Dtos;

/// <summary>
/// DTO class that is used as return type for most of CountriesService methods
/// </summary>
public record CountryResponse
{
	public Guid Id { get; init; }
	public string? Name { get; init; }
}
