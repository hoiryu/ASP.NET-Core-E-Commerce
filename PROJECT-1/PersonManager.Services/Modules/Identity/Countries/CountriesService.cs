using Gridify;
using PersonManager.Entities.Modules.Identity.Countries;
using PersonManager.RepositoryContracts.Modules.Identity.Countries;
using PersonManager.ServiceContracts.Modules.Identity.Countries;
using PersonManager.ServiceContracts.Modules.Identity.Countries.Dtos;
using PersonManager.Services.Common.Helpers;
using PersonManager.Services.Common.Logging;

namespace PersonManager.Services.Modules.Identity.Countries;

public class CountriesService(ICountriesRepository _countriesRepository) : ICountriesService
{
	[LogExecutionTime(nameof(countryCreateRequests))]
	public async Task<List<CountryResponse>> CreateCountries(List<CountryCreateRequest> countryCreateRequests)
	{
		// 저장 전에 전체를 검증 (하나라도 실패하면 아무것도 저장하지 않음)
		ValidationHelper.ModelValidation(countryCreateRequests);

		// 이름 중복(요청 안, DB 에 이미 있는 값)은 유니크 인덱스가 막아 전체 롤백 (UniqueConstraintException → 409)
		List<Country> countries = await _countriesRepository.CreateCountries([
			.. countryCreateRequests.Select(temp => temp.ToEntity()),
		]);

		return [.. countries.Select(country => country.ToResponse())];
	}

	[LogExecutionTime(nameof(query))]
	public async Task<Paging<CountryResponse>> GetCountries(GridifyQuery query)
	{
		Paging<Country> countries = await _countriesRepository.GetCountries(query);

		return new Paging<CountryResponse>(countries.Count, countries.Data.Select(country => country.ToResponse()));
	}

	[LogExecutionTime(nameof(filter))]
	public async Task<int> UpdateCountries(string? filter, CountryUpdateRequest countryUpdateRequest)
	{
		ValidationHelper.FilterValidation(filter);

		ValidationHelper.ModelValidation(countryUpdateRequest);

		return await _countriesRepository.UpdateCountries(filter, countryUpdateRequest.ToUpdate());
	}

	[LogExecutionTime(nameof(filter))]
	public async Task<int> DeleteCountries(string? filter)
	{
		ValidationHelper.FilterValidation(filter);

		return await _countriesRepository.DeleteCountries(filter);
	}
}
