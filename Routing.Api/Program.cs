var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/products/{id=1}", async (HttpContext context, int id) => Results.Ok(new { message = $"ProductId: {id}" }));

app.MapGet(
	"/details/{id?}",
	async (HttpContext context, int? id) =>
		id is null ? Results.Ok(new { message = "No id provided" }) : Results.Ok(new { message = $"DetailId: {id}" })
);

app.MapGet(
	"/daily-digest-report/{reportdate:datetime?}",
	(HttpContext context, DateTime? reportdate) =>
		reportdate is null
			? Results.Ok(new { message = $"Now: {DateTime.Now}" })
			: Results.Ok(new { message = $"reportdate: {reportdate}" })
);

app.MapFallback(
	async (context) =>
		await Results.Ok(new { message = $"Request received at {context.Request.Path}" }).ExecuteAsync(context)
);

app.Run();
