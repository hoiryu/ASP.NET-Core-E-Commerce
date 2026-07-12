namespace MyMiddleware.Api.Middleware;

public class LatestMiddleware(RequestDelegate next)
{
	public async Task InvokeAsync(HttpContext context)
	{
		var messages = context.Items["messages"] as List<string> ?? [];
		context.Items["messages"] = messages;

		messages.Add($"[{messages.Count + 1}] LatestMiddleware");
		await next(context);
	}
}

public static class LatestMiddlewareExtension
{
	public static IApplicationBuilder UseLatestMiddleware(this IApplicationBuilder app)
	{
		return app.UseMiddleware<LatestMiddleware>();
	}
}
