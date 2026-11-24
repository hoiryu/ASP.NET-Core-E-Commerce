using PersonManager.Core.Modules.Identity.Countries.Dtos;
using PersonManager.Domain.Modules.Identity.Countries;

namespace PersonManager.Core.Modules.Identity.Countries;

/// <summary>
/// Country 관련 DTO ↔ Entity 변환
/// </summary>
internal static class CountryMappings
{
	public static Country ToEntity(this CountryCreateRequest request) => new() { Name = request.Name };

	public static CountryUpdate ToUpdate(this CountryUpdateRequest request) => new(request.Name);

	public static CountryResponse ToResponse(this Country country) => new() { Id = country.Id, Name = country.Name };
}
