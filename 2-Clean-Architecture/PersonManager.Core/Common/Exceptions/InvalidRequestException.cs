namespace PersonManager.Core.Common.Exceptions;

/// <summary>
/// 요청 값이 잘못됨 (검증 실패, 빈 filter, 매핑되지 않은 필드, 없는 참조 id, DB 값 오류 등) → 400
/// </summary>
public sealed class InvalidRequestException : Exception
{
	public InvalidRequestException(string message)
		: base(message) { }

	public InvalidRequestException(string message, Exception innerException)
		: base(message, innerException) { }
}
