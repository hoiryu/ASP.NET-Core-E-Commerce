using System.Linq.Expressions;
using ServiceContracts.Common.Dtos;
using ServiceContracts.Common.Enums;

namespace Services.Common.Extensions;

public static class QueryableExtensions
{
	/// <summary>
	/// 정렬 옵션 값이 있으면 첫 번째는 OrderBy, 이후는 ThenBy 로 이어 붙임 (값이 없으면 ordered 그대로 반환)
	/// </summary>
	internal static IOrderedQueryable<T>? OrderByOption<T, U>(
		this IQueryable<T> query,
		IOrderedQueryable<T>? ordered,
		OrderOptions? order,
		Expression<Func<T, U>> keySelector
	)
	{
		if (order is null)
			return ordered;

		bool descending = order == OrderOptions.DESC;

		if (ordered is null)
			return descending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);

		return descending ? ordered.ThenByDescending(keySelector) : ordered.ThenBy(keySelector);
	}

	/// <summary>
	/// 값이 있는 페이징 조건만 Skip / Take 로 이어 붙임 (정렬 이후에 호출해야 순서가 보장됨)
	/// </summary>
	internal static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, Paging paging)
	{
		if (paging.Skip is int skip)
			query = query.Skip(skip);

		if (paging.Take is int take)
			query = query.Take(take);

		return query;
	}
}
