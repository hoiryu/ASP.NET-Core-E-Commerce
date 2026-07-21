using Routing.Api.CustomConstrains;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options =>
{
	options.ConstraintMap.Add("months", typeof(MonthsCustomConstrain));
});

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

app.MapGet(
	"/sales-report/{year:int:min(1900)}/{month:months}",
	(HttpContext context, int year, string month) => Results.Ok(new { message = $"year: {year}, month: {month}" })
);

app.MapFallback(
	async (context) =>
		await Results.Ok(new { message = $"Request received at {context.Request.Path}" }).ExecuteAsync(context)
);

app.Run();
