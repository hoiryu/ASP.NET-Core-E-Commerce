using ServiceContracts.Modules.Countries.Dtos;

namespace ServiceContracts.Modules.Countries;

/// <summary>
/// Represents business login for manipulating Country entity
/// </summary>
public interface ICountriesService
{
	CountryResponse AddCountry(CountryAddRequest? countryAddRequest);

	List<CountryResponse> GetAllCountries();

	CountryResponse? GetCountryByCountryId(Guid? countryId);
}
