using Gridify;
using PersonManager.Entities.Modules.Countries;
using PersonManager.RepositoryContracts.Modules.Countries;
using PersonManager.ServiceContracts.Modules.Countries;
using PersonManager.ServiceContracts.Modules.Countries.Dtos;
using PersonManager.Services.Common.Helpers;

namespace PersonManager.Services.Modules.Countries;

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

	public async Task<CountryResponse?> UpdateCountry(CountryUpdateRequest? countryUpdateRequest)
	{
		ArgumentNullException.ThrowIfNull(countryUpdateRequest);

		ValidationHelper.ModelValidation(countryUpdateRequest);

		Country? matchingCountry = await _countriesRepository.GetCountryById(countryUpdateRequest.Id);

		if (matchingCountry == null)
			return null;

		matchingCountry.Name = countryUpdateRequest.Name ?? matchingCountry.Name;

		Country updatedCountry = await _countriesRepository.UpdateCountry(matchingCountry);

		return updatedCountry.ToResponse();
	}

	public async Task<CountryResponse?> DeleteCountry(Guid? countryId)
	{
		if (!countryId.HasValue)
			throw new ArgumentNullException(nameof(countryId));

		Country? matchingCountry = await _countriesRepository.GetCountryById(countryId.Value);

		if (matchingCountry == null)
			return null;

		Country deletedCountry = await _countriesRepository.DeleteCountry(matchingCountry);

		return deletedCountry.ToResponse();
	}
}
