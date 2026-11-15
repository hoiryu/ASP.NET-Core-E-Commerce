using Gridify;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersonManager.Entities.Data;
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
	/// DbContext, Repository, Repository 가 쓰는 Gridify 매퍼를 등록
	/// 모듈을 추가하면 Repository 와 매퍼를 여기에 함께 등록한다
	/// </summary>
	public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
	{
		// DbContext
		services.AddDbContext<AppDbContext>(options =>
			options
				.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
				.UseSnakeCaseNamingConvention()
				// 실패 예외는 위로 전파되어 ExceptionHandler 가 처리·기록하므로 EF 자체 에러 로그는 Debug 로 낮춤
				.ConfigureWarnings(w =>
					w.Log((RelationalEventId.CommandError, LogLevel.Debug), (CoreEventId.SaveChangesFailed, LogLevel.Debug))
				)
		);

		// Gridify: EF Core 에서 필터 값을 SQL 파라미터로 전달
		GridifyGlobalConfiguration.EnableEntityFrameworkCompatibilityLayer();

		// Countries
		services.AddScoped<ICountriesRepository, CountriesRepository>();
		services.AddSingleton<IGridifyMapper<Country>, CountryGridifyMapper>(); // 상태가 없어 하나만 공유

		// Users
		services.AddScoped<IUsersRepository, UsersRepository>();
		services.AddSingleton<IGridifyMapper<User>, UserGridifyMapper>();

		return services;
	}
}
