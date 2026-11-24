using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PersonManager.Core.Common.Logging;

public static class LoggingServiceCollectionExtensions
{
	// 생성한 프록시 타입을 캐시하므로 하나만 공유
	private static readonly ProxyGenerator Generator = new();

	/// <summary>
	/// 서비스를 Scoped 로 등록하고 로깅 인터셉터를 거치는 프록시로 감싼다
	/// 로그 종류가 늘어나면 인터셉터를 CreateInterfaceProxyWithTarget 인자에 추가한다
	/// </summary>
	public static IServiceCollection AddLoggedScoped<TService, TImplementation>(this IServiceCollection services)
		where TService : class
		where TImplementation : class, TService
	{
		// 상태(메서드별 캐시)만 있어 하나만 공유
		services.TryAddSingleton<ExecutionTimeLoggingInterceptor>();

		services.AddScoped<TImplementation>();
		services.AddScoped(sp =>
			Generator.CreateInterfaceProxyWithTarget<TService>(
				sp.GetRequiredService<TImplementation>(),
				sp.GetRequiredService<ExecutionTimeLoggingInterceptor>()
			)
		);

		return services;
	}
}
