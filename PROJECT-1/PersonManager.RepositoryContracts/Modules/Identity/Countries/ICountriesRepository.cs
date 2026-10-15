using PersonManager.Entities.Modules.Identity.Countries;
using Gridify;
using PersonManager.RepositoryContracts.Modules.Identity.Countries.Dtos;

namespace PersonManager.RepositoryContracts.Modules.Identity.Countries;

public interface ICountriesRepository
{
	Task<Country> CreateCountry(Country country);

	Task<Paging<Country>> GetCountries(GridifyQuery query);

	Task<bool> ExistsCountryById(Guid countryId);

	Task<bool> ExistsCountryByName(string name);

	Task<int> UpdateCountries(string filter, CountryUpdate values);

	Task<int> DeleteCountries(string filter);
}
