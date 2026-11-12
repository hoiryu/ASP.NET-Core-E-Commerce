using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using Castle.DynamicProxy;
using Gridify;
using PersonManager.Services.Common.Attributes;
using SerilogTimings;

namespace PersonManager.Services.Common.Interceptors;

/// <summary>
/// [Timed] 가 붙은 메서드를 SerilogTimings Operation 으로 감싼다
/// 정상 완료면 completed, 예외로 끝나면 abandoned 로 기록되고 예외는 그대로 전파된다
/// </summary>
public sealed class TimingInterceptor : AsyncInterceptorBase
{
	// 구현 메서드별 측정 정보 ([Timed] 가 없으면 null)
	private static readonly ConcurrentDictionary<MethodInfo, TimedMethod?> Cache = new();

	protected override async Task InterceptAsync(
		IInvocation invocation,
		IInvocationProceedInfo proceedInfo,
		Func<IInvocation, IInvocationProceedInfo, Task> proceed
	)
	{
		using var op = Begin(invocation);
		await proceed(invocation, proceedInfo);
		op?.Complete(); // 예외면 도달하지 않아 Dispose 시 abandoned
	}

	protected override async Task<TResult> InterceptAsync<TResult>(
		IInvocation invocation,
		IInvocationProceedInfo proceedInfo,
		Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed
	)
	{
		using var op = Begin(invocation);
		var result = await proceed(invocation, proceedInfo);
		op?.Complete();

		return result;
	}

	private static Operation? Begin(IInvocation invocation)
	{
		// 인터페이스 메서드가 아닌 구현 메서드 기준으로 [Timed] 조회
		var timed = Cache.GetOrAdd(invocation.MethodInvocationTarget ?? invocation.Method, TimedMethod.Create);

		return timed is null ? null : Operation.Begin(timed.Template, timed.GetValues(invocation.Arguments));
	}

	private sealed class TimedMethod(string template, string serviceName, string methodName, int[] parameterIndexes)
	{
		public string Template { get; } = template;

		public static TimedMethod? Create(MethodInfo method)
		{
			var attribute = method.GetCustomAttribute<TimedAttribute>();
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
						$"[Timed] parameter '{name}' not found on {method.DeclaringType?.Name}.{method.Name}"
					);

				var property = PropertyName(parameter);
				template += $" {property}: {{{property}}}";
				indexes.Add(parameter.Position);
			}

			return new TimedMethod(template, method.DeclaringType!.Name, method.Name, [.. indexes]);
		}

		// null 값(빈 filter 등)도 그대로 기록 (SerilogTimings 시그니처가 object[] 라 ! 사용)
		public object[] GetValues(object?[] arguments) =>
			[serviceName, methodName, .. parameterIndexes.Select(index => LogValue(arguments[index])!)];

		// 컬렉션은 건수, Gridify 쿼리는 filter 만 기록
		private static string PropertyName(ParameterInfo parameter) =>
			parameter.ParameterType switch
			{
				var type when typeof(ICollection).IsAssignableFrom(type) => "Count",
				var type when typeof(IGridifyFiltering).IsAssignableFrom(type) => "Filter",
				_ => char.ToUpperInvariant(parameter.Name![0]) + parameter.Name[1..],
			};

		private static object? LogValue(object? value) =>
			value switch
			{
				ICollection collection => collection.Count,
				IGridifyFiltering filtering => filtering.Filter,
				_ => value,
			};
	}
}
