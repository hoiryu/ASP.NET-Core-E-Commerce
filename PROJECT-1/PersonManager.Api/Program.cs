using System.Text.Json.Serialization;
using Gridify;
using Microsoft.EntityFrameworkCore;
using PersonManager.Entities.Data;
using PersonManager.Repositories.Modules.Identity.Countries;
using PersonManager.Repositories.Modules.Identity.Users;
using PersonManager.RepositoryContracts.Modules.Identity.Countries;
using PersonManager.RepositoryContracts.Modules.Identity.Users;
using PersonManager.ServiceContracts.Modules.Identity.Countries;
using PersonManager.ServiceContracts.Modules.Identity.Users;
using PersonManager.Services.Modules.Identity.Countries;
using PersonManager.Services.Modules.Identity.Users;
using Serilog;

// 1단계: 호스트 구성 전에 쓰는 임시 로거 (호스트 구성 중 예외 기록용)
// 호스트가 아직 없으므로 appsettings 를 직접 읽어 파일 저장 설정을 2단계와 공유
var bootstrapConfiguration = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory()) // 호스트의 ContentRoot 와 같은 위치
	.AddJsonFile("appsettings.json", optional: true)
	.AddJsonFile(
		$"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
		optional: true
	)
	.Build();

Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(bootstrapConfiguration)
	.WriteTo.Console()
	.CreateBootstrapLogger();

try
{
	var builder = WebApplication.CreateBuilder(args);

	#region Register Controllers
	builder
		.Services.AddControllers()
		.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())); // Enum 을 문자열로 직렬화
	#endregion

	#region EndPoint 를 kabab-case 로 변환
	builder.Services.AddRouting(o => o.LowercaseUrls = true);
	#endregion

	// builder.Services.AddAppOptions(builder.Configuration); // Options 패턴 바인딩 + 검증

	#region Register with IoC Container
	builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
	builder.Services.AddScoped<IUsersRepository, UsersRepository>();
	builder.Services.AddScoped<ICountriesService, CountriesService>();
	builder.Services.AddScoped<IUsersService, UsersService>();
	#endregion

	#region Gridify
	GridifyGlobalConfiguration.EnableEntityFrameworkCompatibilityLayer(); // EF Core 에서 필터 값을 SQL 파라미터로 전달
	#endregion

	#region Register DbContext
	builder.Services.AddDbContext<AppDbContext>(options =>
		options
			.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
			.UseSnakeCaseNamingConvention()
	);
	#endregion

	#region Serilog
	// 2단계: appsettings·DI 를 반영한 최종 로거로 임시 로거를 대체
	builder.Services.AddSerilog(
		(services, lc) =>
			lc
				.ReadFrom.Configuration(builder.Configuration) // appsettings 의 "Serilog" 섹션 (최소 레벨)
				.ReadFrom.Services(services)
				.Enrich.FromLogContext()
				.WriteTo.Console()
	);
	#endregion

	var app = builder.Build();

	app.UseStaticFiles(); // Static Files
	// UseStaticFiles 보다 뒤에서 처리해서 Static File 로그는 제외
	app.UseSerilogRequestLogging(o => o.IncludeQueryInRequestPath = true); // 요청당 한 줄: 메서드, 경로+쿼리, 상태 코드, 소요 시간
	app.UseRouting(); // Routing 매칭 지점 표시
	app.MapControllers(); // 매칭된 Endpoint 실행

	app.Run();
	return 0;
}
catch (Exception ex) when (ex is not HostAbortedException) // dotnet ef 가 호스트를 중단할 때 던지는 예외는 제외
{
	Log.Fatal(ex, "Application terminated unexpectedly");
	return 1; // 비정상 종료 코드 (컨테이너·오케스트레이터가 실패로 인식)
}
finally
{
	Log.CloseAndFlush(); // 버퍼에 남은 로그를 모두 쓰고 종료
}
