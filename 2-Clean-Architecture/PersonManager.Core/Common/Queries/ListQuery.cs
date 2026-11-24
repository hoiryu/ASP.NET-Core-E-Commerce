namespace PersonManager.Core.Common.Queries;

/// <summary>
/// 목록 조회 조건 (filter·orderBy 는 Gridify 문법 문자열, page 는 1부터)
/// </summary>
public sealed record ListQuery(string? Filter, string? OrderBy, int Page = 1, int PageSize = 20);
