using Gridify;
using PersonManager.ServiceContracts.Modules.Identity.Countries.Dtos;

namespace PersonManager.ServiceContracts.Modules.Identity.Countries;

/// <summary>
/// Represents business login for manipulating Country entity
/// </summary>
public interface ICountriesService
{
	Task<List<CountryResponse>> CreateCountries(List<CountryCreateRequest> countryCreateRequests);

	Task<Paging<CountryResponse>> GetCountries(GridifyQuery query);

	Task<int> UpdateCountries(string? filter, CountryUpdateRequest countryUpdateRequest);

	Task<int> DeleteCountries(string? filter);
}
