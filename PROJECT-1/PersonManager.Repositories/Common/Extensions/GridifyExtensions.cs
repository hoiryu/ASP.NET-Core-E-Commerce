using Gridify;

namespace PersonManager.Repositories.Common.Extensions;

internal static class GridifyExtensions
{
	/// <summary>
	/// mapper 에 없는 필드·잘못된 문법이면 ArgumentException
	/// </summary>
	public static void EnsureValid<T>(this IGridifyQuery query, IGridifyMapper<T> mapper)
	{
		if (!query.IsValid(out var errors, mapper))
			throw new ArgumentException(string.Join(", ", errors));
	}

	/// <summary>
	/// 빈 filter 는 전체 행이 대상이 되므로 거부한 뒤 필터 적용
	/// </summary>
	public static IQueryable<T> ApplyBulkFiltering<T>(this IQueryable<T> source, string filter, IGridifyMapper<T> mapper)
	{
		if (string.IsNullOrWhiteSpace(filter))
			throw new ArgumentException("Filter is required for bulk operations");

		new GridifyQuery { Filter = filter }.EnsureValid(mapper);

		return source.ApplyFiltering(filter, mapper);
	}
}
