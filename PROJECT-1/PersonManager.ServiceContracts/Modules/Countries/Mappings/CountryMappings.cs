using PersonManager.Entities.Modules.Countries;
using PersonManager.RepositoryContracts.Modules.Countries.Dtos;
using PersonManager.ServiceContracts.Modules.Countries.Dtos;

namespace PersonManager.ServiceContracts.Modules.Countries.Mappings;

/// <summary>
/// Country 관련 DTO ↔ Entity 변환
/// </summary>
public static class CountryMappings
{
	public static Country ToEntity(this CountryCreateRequest request) => new() { Name = request.Name };

	public static CountryUpdate ToUpdate(this CountryUpdateRequest request) => new(request.Name);

	public static CountryResponse ToResponse(this Country country) => new() { Id = country.Id, Name = country.Name };
}
