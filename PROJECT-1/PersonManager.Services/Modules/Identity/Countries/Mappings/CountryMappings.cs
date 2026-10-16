using PersonManager.Entities.Modules.Identity.Countries;
using PersonManager.RepositoryContracts.Modules.Identity.Countries.Dtos;
using PersonManager.ServiceContracts.Modules.Identity.Countries.Dtos;

namespace PersonManager.Services.Modules.Identity.Countries.Mappings;

/// <summary>
/// Country 관련 DTO ↔ Entity 변환
/// </summary>
internal static class CountryMappings
{
	public static Country ToEntity(this CountryCreateRequest request) => new() { Name = request.Name };

	public static CountryUpdate ToUpdate(this CountryUpdateRequest request) => new(request.Name);

	public static CountryResponse ToResponse(this Country country) => new() { Id = country.Id, Name = country.Name };
}
