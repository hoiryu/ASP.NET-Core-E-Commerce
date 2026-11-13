using Gridify;
using Microsoft.Extensions.DependencyInjection;
using PersonManager.Entities.Modules.Identity.Countries;
using PersonManager.Entities.Modules.Identity.Users;
using PersonManager.Repositories.Modules.Identity.Countries;
using PersonManager.Repositories.Modules.Identity.Users;
using PersonManager.RepositoryContracts.Modules.Identity.Countries;
using PersonManager.RepositoryContracts.Modules.Identity.Users;

namespace PersonManager.Repositories;

public static class DependencyInjection
{
	/// <summary>
	/// Repository 와 Repository 가 쓰는 Gridify 매퍼를 등록
	/// 모듈을 추가하면 Repository 와 매퍼를 여기에 함께 등록한다
	/// </summary>
	public static IServiceCollection AddRepositories(this IServiceCollection services)
	{
		// Countries
		services.AddScoped<ICountriesRepository, CountriesRepository>();
		services.AddSingleton<IGridifyMapper<Country>, CountryGridifyMapper>(); // 상태가 없어 하나만 공유

		// Users
		services.AddScoped<IUsersRepository, UsersRepository>();
		services.AddSingleton<IGridifyMapper<User>, UserGridifyMapper>();

		return services;
	}
}
