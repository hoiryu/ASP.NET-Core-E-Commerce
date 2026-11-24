using Microsoft.Extensions.DependencyInjection;
using PersonManager.ServiceContracts.Modules.Identity.Countries;
using PersonManager.ServiceContracts.Modules.Identity.Users;
using PersonManager.Services.Common.Logging;
using PersonManager.Services.Modules.Identity.Countries;
using PersonManager.Services.Modules.Identity.Users;

namespace PersonManager.Services;

public static class DependencyInjection
{
	/// <summary>
	/// 서비스를 등록. 로깅 인터셉터([LogExecutionTime] 등)를 거치는 프록시로 감싼다
	/// 모듈을 추가하면 서비스를 여기에 함께 등록한다
	/// </summary>
	public static IServiceCollection AddServices(this IServiceCollection services)
	{
		// Countries
		services.AddLoggedScoped<ICountriesService, CountriesService>();

		// Users
		services.AddLoggedScoped<IUsersService, UsersService>();

		return services;
	}
}
