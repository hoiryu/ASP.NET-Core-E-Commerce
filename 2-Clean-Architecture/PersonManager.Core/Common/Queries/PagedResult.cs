namespace PersonManager.Core.Common.Queries;

/// <summary>
/// 목록 조회 결과: 필터 결과 전체 건수와 현재 페이지 데이터
/// </summary>
public sealed record PagedResult<T>(int Count, IEnumerable<T> Data);
