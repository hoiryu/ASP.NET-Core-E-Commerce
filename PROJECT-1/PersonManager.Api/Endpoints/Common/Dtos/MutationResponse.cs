namespace PersonManager.Api.Endpoints.Common.Dtos;

/// <summary>
/// 일괄 수정·삭제 응답: 영향받은 행 수
/// </summary>
public sealed record MutationResponse(int Count);
