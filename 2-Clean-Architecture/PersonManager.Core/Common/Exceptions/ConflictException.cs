namespace PersonManager.Core.Common.Exceptions;

/// <summary>
/// 요청이 기존 데이터와 충돌함 (유니크 값 중복, 참조 중인 데이터 등) → 409
/// </summary>
public sealed class ConflictException : Exception
{
	public ConflictException(string message)
		: base(message) { }

	public ConflictException(string message, Exception innerException)
		: base(message, innerException) { }
}
