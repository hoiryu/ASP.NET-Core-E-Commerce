using Gridify;
using PersonManager.Entities.Modules.Identity.Countries;
using PersonManager.RepositoryContracts.Modules.Identity.Countries;
using PersonManager.ServiceContracts.Modules.Identity.Countries;
using PersonManager.ServiceContracts.Modules.Identity.Countries.Dtos;
using PersonManager.Services.Common.Helpers;
using PersonManager.Services.Modules.Identity.Countries.Mappings;
using SerilogTimings;

namespace PersonManager.Services.Modules.Identity.Countries;

public class CountriesService(ICountriesRepository _countriesRepository) : ICountriesService
{
	public async Task<List<CountryResponse>> CreateCountries(List<CountryCreateRequest> countryCreateRequests)
	{
		using var _ = Operation.Time(
			"service: {Service:l} method: {Method:l} count: {Count}",
			nameof(CountriesService),
			nameof(CreateCountries),
			countryCreateRequests.Count
		);

		// 저장 전에 전체를 검증 (하나라도 실패하면 아무것도 저장하지 않음)
		ValidationHelper.ModelValidation(countryCreateRequests);

		List<string> names = [.. countryCreateRequests.Select(temp => temp.Name!)];

		// DB collation 과 같게 대소문자를 무시하고 요청 안의 중복 확인
		if (names.Distinct(StringComparer.OrdinalIgnoreCase).Count() != names.Count)
		{
			throw new ArgumentException("Duplicate country names in request");
		}

		if (await _countriesRepository.ExistsAnyCountryByNames(names))
		{
			throw new ArgumentException("Given country name already exists");
		}

		List<Country> countries = await _countriesRepository.CreateCountries([
			.. countryCreateRequests.Select(temp => temp.ToEntity()),
		]);

		return [.. countries.Select(country => country.ToResponse())];
	}

	public async Task<Paging<CountryResponse>> GetCountries(GridifyQuery query)
	{
		using var _ = Operation.Time(
			"service: {Service:l} method: {Method:l} filter: {Filter}",
			nameof(CountriesService),
			nameof(GetCountries),
			query.Filter!
		);

		Paging<Country> countries = await _countriesRepository.GetCountries(query);

		return new Paging<CountryResponse>(countries.Count, countries.Data.Select(country => country.ToResponse()));
	}

	public async Task<int> UpdateCountries(string? filter, CountryUpdateRequest countryUpdateRequest)
	{
		using var _ = Operation.Time(
			"service: {Service:l} method: {Method:l} filter: {Filter}",
			nameof(CountriesService),
			nameof(UpdateCountries),
			filter!
		);

		ValidationHelper.FilterValidation(filter);

		ValidationHelper.ModelValidation(countryUpdateRequest);

		return await _countriesRepository.UpdateCountries(filter, countryUpdateRequest.ToUpdate());
	}

	public async Task<int> DeleteCountries(string? filter)
	{
		using var _ = Operation.Time(
			"Service: {Service:l} Method: {Method:l} Filter: {Filter}",
			nameof(CountriesService),
			nameof(DeleteCountries),
			filter!
		);

		ValidationHelper.FilterValidation(filter);

		return await _countriesRepository.DeleteCountries(filter);
	}
}
