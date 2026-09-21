var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); // Controllers 등록

// builder.Services.AddAppOptions(builder.Configuration); // Options 패턴 바인딩 + 검증
// builder.Services.AddScoped<ICitiesService, CitiesService>(); // Register with IoC Container

var app = builder.Build();

app.UseStaticFiles(); // Static Files 먼저 처리
app.UseRouting(); // Routing 매칭 지점 표시
app.MapControllers(); // 매칭된 Endpoint 실행

app.Run();
