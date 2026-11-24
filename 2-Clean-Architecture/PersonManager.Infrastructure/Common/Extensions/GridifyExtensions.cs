using Gridify;
using PersonManager.Core.Common.Exceptions;
using PersonManager.Core.Common.Queries;

namespace PersonManager.Infrastructure.Common.Extensions;

internal static class GridifyExtensions
{
	/// <summary>
	/// Core 의 목록 조회 조건을 Gridify 쿼리로 변환
	/// orderBy 가 비면 기본 정렬을 넣어 페이징 순서를 고정 (ORDER BY 없는 OFFSET/FETCH 는 순서 보장 안 됨)
	/// </summary>
	public static GridifyQuery ToGridifyQuery(this ListQuery query, string defaultOrderBy = "id") =>
		new()
		{
			Filter = query.Filter,
			OrderBy = string.IsNullOrWhiteSpace(query.OrderBy) ? defaultOrderBy : query.OrderBy,
			Page = query.Page,
			PageSize = query.PageSize,
		};

	public static PagedResult<T> ToPagedResult<T>(this Paging<T> paging) => new(paging.Count, paging.Data);

	/// <summary>
	/// mapper 에 없는 필드·잘못된 문법이면 InvalidRequestException
	/// </summary>
	public static void EnsureValid<T>(this IGridifyQuery query, IGridifyMapper<T> mapper)
	{
		if (!query.IsValid(out var errors, mapper))
			throw new InvalidRequestException(string.Join(", ", errors));
	}

	/// <summary>
	/// filter 를 검증한 뒤 적용 (빈 filter 거부는 Core 의 ValidationHelper.FilterValidation 이 담당)
	/// </summary>
	public static IQueryable<T> ApplyBulkFiltering<T>(
		this IQueryable<T> source,
		string filter,
		IGridifyMapper<T> mapper
	)
	{
		new GridifyQuery { Filter = filter }.EnsureValid(mapper);

		return source.ApplyFiltering(filter, mapper);
	}
}
