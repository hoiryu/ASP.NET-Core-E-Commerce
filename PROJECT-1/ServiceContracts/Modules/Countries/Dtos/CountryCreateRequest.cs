namespace ServiceContracts.Modules.Countries.Dtos;

using Entities.Modules.Countries;

/// <summary>
/// DTO Class for Adding a new country
/// </summary>
public record CountryCreateRequest
{
	public string? Name { get; init; }

	public Country ToEntity() => new() { Name = Name };
}
