using PersonManager.Core.Common.Queries;
using PersonManager.Core.Modules.Identity.Countries.Dtos;
using PersonManager.Domain.Modules.Identity.Countries;

namespace PersonManager.Core.Modules.Identity.Countries;

public interface ICountriesRepository
{
	Task<List<Country>> CreateCountries(List<Country> countries);

	Task<PagedResult<Country>> GetCountries(ListQuery query);

	Task<bool> ExistsCountriesByIds(List<Guid> countryIds);

	Task<int> UpdateCountries(string filter, CountryUpdate values);

	Task<int> DeleteCountries(string filter);
}
