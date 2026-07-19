var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet(
	"/files/{filename=default}.{extension=default}",
	async (HttpContext context, string filename, string extension) =>
		Results.Ok(new { message = $"file: {filename}.{extension}" })
);

app.MapFallback(
	async (context) =>
		await Results.Ok(new { message = $"Request received at {context.Request.Path}" }).ExecuteAsync(context)
);

app.Run();
