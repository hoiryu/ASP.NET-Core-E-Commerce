using Entities.Modules.Countries;
using ServiceContracts.Modules.Countries.Dtos;
using Services.Common.Extensions;

namespace Services.Modules.Countries.Extensions;

public static class CountryQueryExtensions
{
	internal static IQueryable<Country> ApplyFiltering(this IQueryable<Country> countries, CountryFilter filter)
	{
		// 값이 있는 조건만 Where 로 이어 붙임 (AND)
		if (filter.Id is Guid countryId)
			countries = countries.Where(temp => temp.Id == countryId);

		if (!string.IsNullOrEmpty(filter.Name))
			countries = countries.Where(temp => temp.Name != null && temp.Name.Contains(filter.Name));

		return countries;
	}

	internal static IQueryable<Country> ApplyOrdering(this IQueryable<Country> countries, CountryOrder order)
	{
		// 값이 있는 조건만 첫 번째는 OrderBy, 이후는 ThenBy 로 이어 붙임
		IOrderedQueryable<Country>? ordered = null;

		ordered = countries.OrderByOption(ordered, order.Id, temp => temp.Id);
		ordered = countries.OrderByOption(ordered, order.Name, temp => temp.Name);

		return ordered ?? countries;
	}
}
