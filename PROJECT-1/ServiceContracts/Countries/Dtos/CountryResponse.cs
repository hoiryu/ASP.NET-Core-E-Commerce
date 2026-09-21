namespace ServiceContracts.Countries.Dtos;

using Entities.Countries;

/// <summary>
/// DTO class that is used as return type for most of CountriesService methods
/// </summary>
public record CountryResponse
{
	public Guid Id { get; init; }
	public string? Name { get; init; }
}

public static class CountryExtensions
{
	public static CountryResponse ToResponse(this Country country) => new() { Id = country.Id, Name = country.Name };
}
