var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWhen(
	context => context.Request.Query.ContainsKey("username"),
	app =>
	{
		app.Use(
			async (HttpContext context, RequestDelegate next) =>
			{
				var username = context.Request.Query["username"].ToString();
				await Results
					.Ok(new { username = string.IsNullOrEmpty(username) ? "unknown" : username })
					.ExecuteAsync(context);
			}
		);
	}
);

app.Run();
