namespace PersonManager.ServiceContracts.Modules.Countries.Dtos;

using PersonManager.Entities.Modules.Countries;

public record CountryCreateRequest
{
	public string? Name { get; init; }

	public Country ToEntity() => new() { Name = Name };
}
