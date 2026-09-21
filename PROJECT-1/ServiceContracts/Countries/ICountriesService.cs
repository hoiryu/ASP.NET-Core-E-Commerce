using ServiceContracts.Countries.Dtos;

namespace ServiceContracts.Countries;

/// <summary>
/// Represents business login for manipulating Country entity
/// </summary>
public interface ICountriesService
{
	CountryResponse AddCountry(CountryAddRequest? countryAddRequest);

	List<CountryResponse> GetAllCountries();

	CountryResponse? GetCountryByCountryId(Guid? countryId);
}
