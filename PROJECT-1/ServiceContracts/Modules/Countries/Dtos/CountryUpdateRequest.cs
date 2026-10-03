namespace ServiceContracts.Modules.Countries.Dtos;

public record CountryUpdateRequest
{
	public Guid Id { get; set; }

	public string? Name { get; set; }
}
