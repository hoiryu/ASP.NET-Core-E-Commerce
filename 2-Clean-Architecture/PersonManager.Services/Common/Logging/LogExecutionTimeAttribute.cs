namespace PersonManager.Services.Common.Logging;

/// <summary>
/// 서비스 메서드 실행 시간을 측정해 로그로 남긴다 (ExecutionTimeLoggingInterceptor 가 처리)
/// 구현 클래스의 메서드에 붙이고, DI 로 받은 인터페이스를 통한 호출만 측정된다 (자기 호출·직접 new 는 측정 안 됨)
/// </summary>
/// <param name="parameters">로그 속성으로 남길 파라미터 이름 (nameof 로 지정)</param>
[AttributeUsage(AttributeTargets.Method)]
public sealed class LogExecutionTimeAttribute(params string[] parameters) : Attribute
{
	public string[] Parameters { get; } = parameters;
}
