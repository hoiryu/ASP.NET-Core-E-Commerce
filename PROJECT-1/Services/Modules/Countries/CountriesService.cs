using System.Linq.Expressions;
using Entities.Data;
using Entities.Modules.Countries;
using ServiceContracts.Common.Enums;
using ServiceContracts.Modules.Countries;
using ServiceContracts.Modules.Countries.Dtos;

namespace Services.Modules.Countries;

public class CountriesService(AppDbContext _db) : ICountriesService
{
	public CountryResponse CreateCountry(CountryCreateRequest? countryCreateRequest)
	{
		if (countryCreateRequest == null || countryCreateRequest.Name == null)
		{
			throw new ArgumentException("countryCreateRequest and countryCreateRequest.Name must not be null");
		}

		if (_db.Countries.Any(temp => temp.Name == countryCreateRequest.Name))
		{
			throw new ArgumentException("Given country name already exists");
		}

		Country country = countryCreateRequest.ToEntity();

		_db.Countries.Add(country);
		_db.SaveChanges();

		return country.ToResponse();
	}

	public List<CountryResponse> GetCountries(CountryFilter filter, CountrySort sort)
	{
		IQueryable<Country> countries = _db.Countries;

		countries = FilterCountries(countries, filter);
		countries = SortCountries(countries, sort);

		// 여기서 SQL 실행 (WHERE + ORDER BY 포함)
		return [.. countries.Select(country => country.ToResponse())];
	}

	private static IQueryable<Country> FilterCountries(IQueryable<Country> countries, CountryFilter filter)
	{
		// 값이 있는 조건만 Where 로 이어 붙임 (AND)
		if (filter.Id is Guid countryId)
			countries = countries.Where(temp => temp.Id == countryId);

		if (!string.IsNullOrEmpty(filter.Name))
			countries = countries.Where(temp => temp.Name != null && temp.Name.Contains(filter.Name));

		return countries;
	}

	private static IQueryable<Country> SortCountries(IQueryable<Country> countries, CountrySort sort)
	{
		// 값이 있는 조건만 첫 번째는 OrderBy, 이후는 ThenBy 로 이어 붙임
		IOrderedQueryable<Country>? ordered = null;

		ordered = ApplySort(countries, ordered, sort.Id, temp => temp.Id);
		ordered = ApplySort(countries, ordered, sort.Name, temp => temp.Name);

		return ordered ?? countries;
	}

	private static IOrderedQueryable<Country>? ApplySort<TKey>(
		IQueryable<Country> countries,
		IOrderedQueryable<Country>? ordered,
		SortOrderOptions? sortOrder,
		Expression<Func<Country, TKey>> keySelector
	)
	{
		if (sortOrder is null)
			return ordered;

		bool descending = sortOrder == SortOrderOptions.DESC;

		if (ordered is null)
			return descending ? countries.OrderByDescending(keySelector) : countries.OrderBy(keySelector);

		return descending ? ordered.ThenByDescending(keySelector) : ordered.ThenBy(keySelector);
	}
}
