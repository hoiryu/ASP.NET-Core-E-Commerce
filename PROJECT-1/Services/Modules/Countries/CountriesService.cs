using Entities.Modules.Countries;
using ServiceContracts.Common.Dtos;
using ServiceContracts.Common.Enums;
using ServiceContracts.Modules.Countries;
using ServiceContracts.Modules.Countries.Dtos;
using ServiceContracts.Modules.Countries.Enums;

namespace Services.Modules.Countries;

public class CountriesService(bool initialize = true) : ICountriesService
{
	private readonly List<Country> _countries = initialize ? CreateMockData() : [];

	private static List<Country> CreateMockData()
	{
		List<string> sourceArray = ["Korea", "Japan", "USA", "India", "Australia"];
		return [.. sourceArray.Select(name => new Country { Id = Guid.NewGuid(), Name = name })];
	}

	public CountryResponse CreateCountry(CountryCreateRequest? countryCreateRequest)
	{
		if (countryCreateRequest == null || countryCreateRequest.Name == null)
		{
			throw new ArgumentException("countryCreateRequest and countryCreateRequest.Name must not be null");
		}

		if (_countries.Any(temp => temp.Name == countryCreateRequest.Name))
		{
			throw new ArgumentException("Given country name already exists");
		}

		Country country = countryCreateRequest.ToEntity();

		country.Id = Guid.NewGuid();

		_countries.Add(country);

		return country.ToResponse();
	}

	public List<CountryResponse> GetCountries(SearchQuery<CountrySearchOptions> query)
	{
		List<CountryResponse> countries = [.. _countries.Select(country => country.ToResponse())];

		return SortCountries(FilterCountries(countries, query), query);
	}

	private static List<CountryResponse> FilterCountries(
		List<CountryResponse> countries,
		SearchQuery<CountrySearchOptions> query
	)
	{
		string? searchString = query.SearchString;

		if (query.SearchBy is null || string.IsNullOrEmpty(searchString))
			return countries;

		return query.SearchBy switch
		{
			CountrySearchOptions.Id =>
			[
				.. countries.Where(temp => Guid.TryParse(searchString, out Guid countryId) && temp.Id == countryId),
			],

			CountrySearchOptions.Name =>
			[
				.. countries.Where(temp => temp.Name?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true),
			],

			_ => countries,
		};
	}

	private static List<CountryResponse> SortCountries(
		List<CountryResponse> countries,
		SearchQuery<CountrySearchOptions> query
	)
	{
		if (query.SortBy is null || !Enum.IsDefined(query.SortOrder))
			return countries;

		return (query.SortBy, query.SortOrder) switch
		{
			(CountrySearchOptions.Id, SortOrderOptions.ASC) => [.. countries.OrderBy(temp => temp.Id)],

			(CountrySearchOptions.Id, SortOrderOptions.DESC) => [.. countries.OrderByDescending(temp => temp.Id)],

			(CountrySearchOptions.Name, SortOrderOptions.ASC) =>
			[
				.. countries.OrderBy(temp => temp.Name, StringComparer.OrdinalIgnoreCase),
			],

			(CountrySearchOptions.Name, SortOrderOptions.DESC) =>
			[
				.. countries.OrderByDescending(temp => temp.Name, StringComparer.OrdinalIgnoreCase),
			],

			_ => countries,
		};
	}
}
