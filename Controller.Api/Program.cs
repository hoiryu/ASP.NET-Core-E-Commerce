using System.Text.Json;
using Controller.Api.CustomModelBinders;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder
	.Services.AddControllers(options =>
	{
		options.ModelBinderProviders.Insert(0, new PersonModelBinderProvider());
	})
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
	});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
	options.InvalidModelStateResponseFactory = ctx =>
	{
		var errors = ctx
			.ModelState.Where(e => e.Value?.Errors.Count > 0)
			.ToDictionary(kvp => kvp.Key, kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

		var response = new
		{
			Status = StatusCodes.Status400BadRequest,
			Message = "Invalid input.",
			Errors = errors,
		};

		return new BadRequestObjectResult(response);
	};
});

var app = builder.Build();

app.UseStaticFiles();

app.MapControllers();

app.Run();
