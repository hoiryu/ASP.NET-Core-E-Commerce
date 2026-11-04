using Gridify;
using PersonManager.Entities.Modules.Identity.Countries;
using PersonManager.RepositoryContracts.Modules.Identity.Countries.Dtos;

namespace PersonManager.RepositoryContracts.Modules.Identity.Countries;

public interface ICountriesRepository
{
	Task<List<Country>> CreateCountries(List<Country> countries);

	Task<Paging<Country>> GetCountries(GridifyQuery query);

	Task<bool> ExistsCountryById(Guid countryId);

	Task<bool> ExistsCountriesByIds(List<Guid> countryIds);

	Task<bool> ExistsAnyCountryByNames(List<string> names);

	Task<int> UpdateCountries(string filter, CountryUpdate values);

	Task<int> DeleteCountries(string filter);
}
