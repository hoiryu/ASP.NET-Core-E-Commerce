using Entities.Countries;
using ServiceContracts.Countries;
using ServiceContracts.Countries.Dtos;

namespace Services.Countries;

public class CountriesService : ICountriesService
{
	private readonly List<Country> _countries = [];

	public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
	{
		if (countryAddRequest == null || countryAddRequest.Name == null)
		{
			throw new ArgumentException("countryAddRequest and countryAddRequest.Name must not be null");
		}

		if (_countries.Any(temp => temp.Name == countryAddRequest.Name))
		{
			throw new ArgumentException("Given country name already exists");
		}

		Country country = countryAddRequest.ToEntity();

		country.Id = Guid.NewGuid();

		_countries.Add(country);

		return country.ToResponse();
	}

	public List<CountryResponse> GetAllCountries()
	{
		return [.. _countries.Select(country => country.ToResponse())];
	}

	public CountryResponse? GetCountryByCountryId(Guid? countryId)
	{
		if (countryId == null)
			return null;

		return _countries.FirstOrDefault(temp => temp.Id == countryId)?.ToResponse();
	}
}
