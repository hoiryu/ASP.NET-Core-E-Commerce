using Gridify;

namespace PersonManager.Api.Endpoints.Common;

/// <summary>
/// 목록 조회 쿼리스트링 (filter, orderBy, page, pageSize) 바인딩용
/// GridifyQuery 를 [AsParameters] 로 직접 받으면 int 속성(page, pageSize)이 필수 파라미터가 되므로 nullable 로 받는다
/// </summary>
public sealed class GridifyQueryParameters
{
	public string? Filter { get; init; }
	public string? OrderBy { get; init; }
	public int? Page { get; init; }
	public int? PageSize { get; init; }

	/// <summary>
	/// 값이 없는 page, pageSize 는 GridifyQuery 기본값(1, 20)을 유지
	/// </summary>
	public GridifyQuery ToGridifyQuery()
	{
		var query = new GridifyQuery { Filter = Filter, OrderBy = OrderBy };

		if (Page is int page)
			query.Page = page;
		if (PageSize is int pageSize)
			query.PageSize = pageSize;

		return query;
	}
}
