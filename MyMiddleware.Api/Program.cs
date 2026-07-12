using MyMiddleware.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<LegacyMiddleware>();

var app = builder.Build();

app.UseLegacyMiddleware();
app.UseLatestMiddleware();

app.MapGet(
	"/",
	(HttpContext context) =>
	{
		var messages = context.Items["messages"] as List<string> ?? [];
		messages.Add("End");
		return Results.Json(new { messages });
	}
);

app.Run();
