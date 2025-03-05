namespace ModularMonolith.Web.Configuration;

public class ContentTypeOptionsMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
            return Task.CompletedTask;
        });

        await next(context);
    }
}
