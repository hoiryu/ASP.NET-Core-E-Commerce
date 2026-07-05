var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet(
	"/ex1",
	async (string? name) =>
	{
		var result = new Dictionary<string, string> { { "name", name ?? "no name" } };
		return Results.Ok(result);
	}
);

app.MapGet(
	"/ex2",
	async (string? name) =>
	{
		return Results.Ok(new { name = name ?? "no name" });
	}
);

app.Run();
