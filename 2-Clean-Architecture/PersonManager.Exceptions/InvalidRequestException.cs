namespace PersonManager.Exceptions;

/// <summary>
/// 요청 값이 잘못됨 (검증 실패, 빈 filter, 매핑되지 않은 필드, 없는 참조 id 등)
/// </summary>
public sealed class InvalidRequestException(string message) : Exception(message);
