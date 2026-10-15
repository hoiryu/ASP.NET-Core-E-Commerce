using Gridify;
using PersonManager.Entities.Modules.Identity.Countries;
using PersonManager.RepositoryContracts.Modules.Identity.Countries;
using PersonManager.ServiceContracts.Modules.Identity.Countries;
using PersonManager.ServiceContracts.Modules.Identity.Countries.Dtos;
using PersonManager.ServiceContracts.Modules.Identity.Countries.Mappings;
using PersonManager.Services.Common.Helpers;

namespace PersonManager.Services.Modules.Identity.Countries;

public class CountriesService(ICountriesRepository _countriesRepository) : ICountriesService
{
	public async Task<CountryResponse> CreateCountry(CountryCreateRequest? countryCreateRequest)
	{
		if (countryCreateRequest == null || countryCreateRequest.Name == null)
		{
			throw new ArgumentException("countryCreateRequest and countryCreateRequest.Name must not be null");
		}

		if (await _countriesRepository.ExistsCountryByName(countryCreateRequest.Name))
		{
			throw new ArgumentException("Given country name already exists");
		}

		Country country = await _countriesRepository.CreateCountry(countryCreateRequest.ToEntity());

		return country.ToResponse();
	}

	public async Task<Paging<CountryResponse>> GetCountries(GridifyQuery query)
	{
		Paging<Country> countries = await _countriesRepository.GetCountries(query);

		return new Paging<CountryResponse>(countries.Count, countries.Data.Select(country => country.ToResponse()));
	}

	public async Task<int> UpdateCountries(string? filter, CountryUpdateRequest? countryUpdateRequest)
	{
		ArgumentNullException.ThrowIfNull(countryUpdateRequest);

		ValidationHelper.ModelValidation(countryUpdateRequest);

		return await _countriesRepository.UpdateCountries(filter ?? "", countryUpdateRequest.ToUpdate());
	}

	public async Task<int> DeleteCountries(string? filter)
	{
		return await _countriesRepository.DeleteCountries(filter ?? "");
	}
}
