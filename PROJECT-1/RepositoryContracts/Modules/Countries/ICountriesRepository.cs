using Entities.Modules.Countries;
using Gridify;

namespace RepositoryContracts.Modules.Countries;

public interface ICountriesRepository
{
	Task<Country> CreateCountry(Country country);

	Task<Paging<Country>> GetCountries(GridifyQuery query);

	Task<Country> UpdateCountry(Country country);

	Task<Country?> DeleteCountry(Guid countryId);
}
