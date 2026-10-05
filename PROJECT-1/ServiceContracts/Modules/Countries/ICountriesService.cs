using ServiceContracts.Common.Dtos;
using ServiceContracts.Modules.Countries.Dtos;

namespace ServiceContracts.Modules.Countries;

/// <summary>
/// Represents business login for manipulating Country entity
/// </summary>
public interface ICountriesService
{
	CountryResponse CreateCountry(CountryCreateRequest? countryCreateRequest);

	List<CountryResponse> GetCountries(CountryFilter filter, CountryOrder order, Paging paging);

	CountryResponse? UpdateCountry(CountryUpdateRequest? countryUpdateRequest);

	CountryResponse? DeleteCountry(Guid? countryId);
}
