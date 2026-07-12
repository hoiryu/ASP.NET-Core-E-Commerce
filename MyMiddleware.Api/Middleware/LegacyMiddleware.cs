namespace MyMiddleware.Api.Middleware;

public class LegacyMiddleware : IMiddleware
{
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		var messages = context.Items["messages"] as List<string> ?? [];
		context.Items["messages"] = messages;

		messages.Add($"[{messages.Count + 1}] LegacyMiddleware");
		await next(context);
	}
}

public static class LegacyMiddlewareExtension
{
	public static IApplicationBuilder UseLegacyMiddleware(this IApplicationBuilder app)
	{
		return app.UseMiddleware<LegacyMiddleware>();
	}
}
