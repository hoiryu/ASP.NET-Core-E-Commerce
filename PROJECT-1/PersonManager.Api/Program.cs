using System.Text.Json.Serialization;
using Entities.Data;
using Microsoft.EntityFrameworkCore;
using ServiceContracts.Modules.Countries;
using ServiceContracts.Modules.Users;
using Services.Modules.Countries;
using Services.Modules.Users;

var builder = WebApplication.CreateBuilder(args);

#region Register Controllers
builder
	.Services.AddControllers()
	.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())); // enum 을 문자열로 직렬화
#endregion

#region EndPoint 를 kabab-case 로 변환
builder.Services.AddRouting(o => o.LowercaseUrls = true);
#endregion

// builder.Services.AddAppOptions(builder.Configuration); // Options 패턴 바인딩 + 검증

#region Register with IoC Container
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IUsersService, UsersService>();
#endregion

#region Register DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")).UseSnakeCaseNamingConvention()
);
#endregion

var app = builder.Build();

app.UseStaticFiles(); // Static Files 먼저 처리
app.UseRouting(); // Routing 매칭 지점 표시
app.MapControllers(); // 매칭된 Endpoint 실행

app.Run();
