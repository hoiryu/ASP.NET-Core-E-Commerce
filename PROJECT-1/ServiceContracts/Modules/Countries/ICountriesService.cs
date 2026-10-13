using Gridify;
using ServiceContracts.Modules.Countries.Dtos;

namespace ServiceContracts.Modules.Countries;

/// <summary>
/// Represents business login for manipulating Country entity
/// </summary>
public interface ICountriesService
{
	Task<CountryResponse> CreateCountry(CountryCreateRequest? countryCreateRequest);

	Task<Paging<CountryResponse>> GetCountries(GridifyQuery query);

	Task<CountryResponse?> UpdateCountry(CountryUpdateRequest? countryUpdateRequest);

	Task<CountryResponse?> DeleteCountry(Guid? countryId);
}
