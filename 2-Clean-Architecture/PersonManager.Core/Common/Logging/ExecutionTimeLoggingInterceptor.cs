using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using PersonManager.Core.Common.Queries;

namespace PersonManager.Core.Common.Logging;

/// <summary>
/// [LogExecutionTime] 이 붙은 메서드의 실행 시간을 ILogger 로 기록한다
/// 정상 완료면 completed(Information), 예외로 끝나면 abandoned(Warning) 로 기록되고 예외는 그대로 전파된다
/// 메시지 형식은 "{템플릿} {Outcome} in {Elapsed:0.0} ms" (기존 SerilogTimings 와 같은 속성 이름)
/// </summary>
public sealed class ExecutionTimeLoggingInterceptor(ILoggerFactory loggerFactory) : AsyncInterceptorBase
{
	// 구현 메서드별 측정 정보 ([LogExecutionTime] 이 없으면 null)
	private readonly ConcurrentDictionary<MethodInfo, LoggedMethod?> _cache = new();

	protected override async Task InterceptAsync(
		IInvocation invocation,
		IInvocationProceedInfo proceedInfo,
		Func<IInvocation, IInvocationProceedInfo, Task> proceed
	)
	{
		var operation = Begin(invocation);
		try
		{
			await proceed(invocation, proceedInfo);
			operation?.Complete();
		}
		catch
		{
			operation?.Abandon();
			throw;
		}
	}

	protected override async Task<TResult> InterceptAsync<TResult>(
		IInvocation invocation,
		IInvocationProceedInfo proceedInfo,
		Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed
	)
	{
		var operation = Begin(invocation);
		try
		{
			var result = await proceed(invocation, proceedInfo);
			operation?.Complete();

			return result;
		}
		catch
		{
			operation?.Abandon();
			throw;
		}
	}

	private Operation? Begin(IInvocation invocation)
	{
		// 인터페이스 메서드가 아닌 구현 메서드 기준으로 [LogExecutionTime] 조회
		var loggedMethod = _cache.GetOrAdd(
			invocation.MethodInvocationTarget ?? invocation.Method,
			method => LoggedMethod.Create(method, loggerFactory)
		);

		return loggedMethod is null ? null : new Operation(loggedMethod, loggedMethod.GetValues(invocation.Arguments));
	}

	/// <summary>
	/// 메서드 호출 한 번의 측정
	/// </summary>
	private sealed class Operation(LoggedMethod method, object?[] values)
	{
		private readonly long _startTimestamp = Stopwatch.GetTimestamp();

		public void Complete() => Write(LogLevel.Information, "completed");

		public void Abandon() => Write(LogLevel.Warning, "abandoned");

		private void Write(LogLevel level, string outcome)
		{
			if (!method.Logger.IsEnabled(level))
				return;

			var elapsed = Stopwatch.GetElapsedTime(_startTimestamp).TotalMilliseconds;

#pragma warning disable CA2254 // 템플릿은 메서드별로 한 번 만들어 캐시하므로 상수가 아니어도 됨
			method.Logger.Log(level, method.Template, [.. values, outcome, elapsed]);
#pragma warning restore CA2254
		}
	}

	private sealed class LoggedMethod(
		ILogger logger,
		string template,
		string serviceName,
		string methodName,
		int[] parameterIndexes
	)
	{
		public ILogger Logger { get; } = logger;

		public string Template { get; } = template;

		public static LoggedMethod? Create(MethodInfo method, ILoggerFactory loggerFactory)
		{
			var attribute = method.GetCustomAttribute<LogExecutionTimeAttribute>();
			if (attribute is null)
				return null;

			var parameters = method.GetParameters();
			var template = "Service: {Service} Method: {Method}";
			List<int> indexes = [];

			foreach (var name in attribute.Parameters)
			{
				var parameter =
					parameters.FirstOrDefault(temp => temp.Name == name)
					?? throw new InvalidOperationException(
						$"[LogExecutionTime] parameter '{name}' not found on {method.DeclaringType?.Name}.{method.Name}"
					);

				var property = PropertyName(parameter);
				template += $" {property}: {{{property}}}";
				indexes.Add(parameter.Position);
			}

			template += " {Outcome} in {Elapsed:0.0} ms";

			var declaringType = method.DeclaringType!;

			return new LoggedMethod(
				loggerFactory.CreateLogger(declaringType.FullName ?? declaringType.Name),
				template,
				declaringType.Name,
				method.Name,
				[.. indexes]
			);
		}

		// null 값(빈 filter 등)도 그대로 기록
		public object?[] GetValues(object?[] arguments) =>
			[serviceName, methodName, .. parameterIndexes.Select(index => LogValue(arguments[index]))];

		// 컬렉션은 건수, 목록 조회 조건은 filter 만 기록
		private static string PropertyName(ParameterInfo parameter) =>
			parameter.ParameterType switch
			{
				var type when typeof(ICollection).IsAssignableFrom(type) => "Count",
				var type when type == typeof(ListQuery) => "Filter",
				_ => char.ToUpperInvariant(parameter.Name![0]) + parameter.Name[1..],
			};

		private static object? LogValue(object? value) =>
			value switch
			{
				ICollection collection => collection.Count,
				ListQuery query => query.Filter,
				_ => value,
			};
	}
}
