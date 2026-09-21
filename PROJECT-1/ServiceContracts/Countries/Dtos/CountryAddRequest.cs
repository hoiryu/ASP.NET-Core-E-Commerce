namespace ServiceContracts.Countries.Dtos;

using Entities.Countries;

/// <summary>
/// DTO Class for Adding a new country
/// </summary>
public record CountryAddRequest
{
	public string? Name { get; init; }

	public Country ToEntity() => new() { Name = Name };
}
