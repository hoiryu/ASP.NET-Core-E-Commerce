using EntityFramework.Exceptions.SqlServer;
using Gridify;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersonManager.Core.Modules.Identity.Countries;
using PersonManager.Core.Modules.Identity.Users;
using PersonManager.Domain.Modules.Identity.Countries;
using PersonManager.Domain.Modules.Identity.Users;
using PersonManager.Infrastructure.Data;
using PersonManager.Infrastructure.Modules.Identity.Countries;
using PersonManager.Infrastructure.Modules.Identity.Users;

namespace PersonManager.Infrastructure;

public static class DependencyInjection
{
	/// <summary>
	/// DbContext, Repository, Repository 가 쓰는 Gridify 매퍼를 등록 (Core 의 Repository 인터페이스 구현)
	/// 모듈을 추가하면 Repository 와 매퍼를 여기에 함께 등록한다
	/// </summary>
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		// DbContext
		services.AddDbContext<AppDbContext>(options =>
			options
				.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
				.UseSnakeCaseNamingConvention()
				// DB 오류를 타입 예외로 변환 (유니크 위반 → UniqueConstraintException 등). Repository 가 다시 Core 예외로 변환 (DbExceptionExtensions)
				.UseExceptionProcessor()
				// 실패 예외는 위로 전파되어 ExceptionHandler 가 처리·기록하므로 EF 자체 에러 로그는 Debug 로 낮춤
				.ConfigureWarnings(w =>
					w.Log(
						(RelationalEventId.CommandError, LogLevel.Debug),
						(CoreEventId.SaveChangesFailed, LogLevel.Debug)
					)
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
