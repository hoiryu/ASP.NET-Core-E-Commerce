using Entities.Modules.Countries;
using Gridify;
using RepositoryContracts.Modules.Countries;
using ServiceContracts.Modules.Countries;
using ServiceContracts.Modules.Countries.Dtos;
using Services.Common.Helpers;

namespace Services.Modules.Countries;

public class CountriesService(ICountriesRepository _countriesRepository) : ICountriesService
{
	public async Task<CountryResponse> CreateCountry(CountryCreateRequest? countryCreateRequest)
	{
		if (countryCreateRequest == null || countryCreateRequest.Name == null)
		{
			throw new ArgumentException("countryCreateRequest and countryCreateRequest.Name must not be null");
		}

		Paging<Country> sameNameCountries = await _countriesRepository.GetCountries(
			new GridifyQuery { Filter = $"name={GridifyHelper.Escape(countryCreateRequest.Name)}", PageSize = 1 }
		);

		if (sameNameCountries.Count > 0)
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

		Country? matchingCountry = (
			await _countriesRepository.GetCountries(new GridifyQuery { Filter = $"id={countryUpdateRequest.Id}", PageSize = 1 })
		).Data.FirstOrDefault();

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

		Country? deletedCountry = await _countriesRepository.DeleteCountry(countryId.Value);

		return deletedCountry?.ToResponse();
	}
}
