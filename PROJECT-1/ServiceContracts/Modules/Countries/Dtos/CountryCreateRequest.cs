namespace ServiceContracts.Modules.Countries.Dtos;

using Entities.Modules.Countries;

public record CountryCreateRequest
{
	public string? Name { get; init; }

	public Country ToEntity() => new() { Name = Name };
}
