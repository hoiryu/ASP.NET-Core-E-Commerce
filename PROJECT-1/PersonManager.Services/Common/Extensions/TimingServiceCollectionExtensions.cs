using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using PersonManager.Services.Common.Interceptors;

namespace PersonManager.Services.Common.Extensions;

public static class TimingServiceCollectionExtensions
{
	// 생성한 프록시 타입을 캐시하므로 하나만 공유
	private static readonly ProxyGenerator Generator = new();

	// 상태가 없어 공유 가능
	private static readonly TimingInterceptor Interceptor = new();

	/// <summary>
	/// 서비스를 Scoped 로 등록하고 [Timed] 메서드를 측정하는 프록시로 감싼다
	/// </summary>
	public static IServiceCollection AddTimedScoped<TService, TImplementation>(this IServiceCollection services)
		where TService : class
		where TImplementation : class, TService
	{
		services.AddScoped<TImplementation>();
		services.AddScoped(sp =>
			Generator.CreateInterfaceProxyWithTarget<TService>(sp.GetRequiredService<TImplementation>(), Interceptor)
		);

		return services;
	}
}
