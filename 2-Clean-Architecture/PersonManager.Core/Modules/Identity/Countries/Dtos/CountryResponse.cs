namespace PersonManager.Core.Modules.Identity.Countries.Dtos;

public record CountryResponse
{
	public Guid Id { get; init; }
	public string? Name { get; init; }
}
