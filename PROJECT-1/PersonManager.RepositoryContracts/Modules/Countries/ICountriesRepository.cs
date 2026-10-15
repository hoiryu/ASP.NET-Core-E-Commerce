using PersonManager.Entities.Modules.Countries;
using Gridify;

namespace PersonManager.RepositoryContracts.Modules.Countries;

public interface ICountriesRepository
{
	Task<Country> CreateCountry(Country country);

	Task<Paging<Country>> GetCountries(GridifyQuery query);

	Task<Country?> GetCountryById(Guid countryId);

	Task<bool> ExistsCountryById(Guid countryId);

	Task<bool> ExistsCountryByName(string name);

	Task<Country> UpdateCountry(Country country);

	Task<Country> DeleteCountry(Country country);
}
