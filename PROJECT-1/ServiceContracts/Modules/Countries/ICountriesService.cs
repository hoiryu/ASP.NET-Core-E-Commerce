using ServiceContracts.Common.Dtos;
using ServiceContracts.Modules.Countries.Dtos;
using ServiceContracts.Modules.Countries.Enums;

namespace ServiceContracts.Modules.Countries;

/// <summary>
/// Represents business login for manipulating Country entity
/// </summary>
public interface ICountriesService
{
	CountryResponse CreateCountry(CountryCreateRequest? countryCreateRequest);

	List<CountryResponse> GetCountries(SearchQuery<CountrySearchOptions> query);
}
