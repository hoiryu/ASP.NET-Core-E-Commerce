using PersonManager.Core.Common.Queries;

namespace PersonManager.Presentation.Endpoints.Common.Parameters;

/// <summary>
/// 목록 조회 쿼리스트링 (filter, orderBy, page, pageSize) 바인딩용
/// page, pageSize 가 필수 파라미터가 되지 않도록 nullable 로 받는다
/// </summary>
public sealed class ListQueryParameters
{
	public string? Filter { get; init; }
	public string? OrderBy { get; init; }
	public int? Page { get; init; }
	public int? PageSize { get; init; }

	/// <summary>
	/// 값이 없는 page, pageSize 는 ListQuery 기본값(1, 20)을 유지
	/// </summary>
	public ListQuery ToListQuery()
	{
		var query = new ListQuery(Filter, OrderBy);

		if (Page is int page)
			query = query with { Page = page };
		if (PageSize is int pageSize)
			query = query with { PageSize = pageSize };

		return query;
	}
}
