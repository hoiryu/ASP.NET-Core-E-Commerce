using PersonManager.Core.Common.Queries;
using PersonManager.Core.Modules.Identity.Countries.Dtos;

namespace PersonManager.Core.Modules.Identity.Countries;

/// <summary>
/// Country 유스케이스 (생성·조회·일괄 수정·일괄 삭제)
/// </summary>
public interface ICountriesService
{
	Task<List<CountryResponse>> CreateCountries(List<CountryCreateRequest> countryCreateRequests);

	Task<PagedResult<CountryResponse>> GetCountries(ListQuery query);

	Task<int> UpdateCountries(string? filter, CountryUpdateRequest countryUpdateRequest);

	Task<int> DeleteCountries(string? filter);
}
