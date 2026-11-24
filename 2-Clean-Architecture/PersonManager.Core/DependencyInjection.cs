using Microsoft.Extensions.DependencyInjection;
using PersonManager.Core.Common.Logging;
using PersonManager.Core.Modules.Identity.Countries;
using PersonManager.Core.Modules.Identity.Users;

namespace PersonManager.Core;

public static class DependencyInjection
{
	/// <summary>
	/// 서비스를 등록. 로깅 인터셉터([LogExecutionTime] 등)를 거치는 프록시로 감싼다
	/// 모듈을 추가하면 서비스를 여기에 함께 등록한다
	/// </summary>
	public static IServiceCollection AddCore(this IServiceCollection services)
	{
		// 현재 시각 (테스트에서는 FakeTimeProvider 로 교체)
		services.AddSingleton(TimeProvider.System);

		// Countries
		services.AddLoggedScoped<ICountriesService, CountriesService>();

		// Users
		services.AddLoggedScoped<IUsersService, UsersService>();

		return services;
	}
}
