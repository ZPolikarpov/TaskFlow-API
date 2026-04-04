using System.Diagnostics;

namespace TaskFlow.Api.Middleware;

public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    public RequestTimingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext httpContext,
        ILogger<RequestTimingMiddleware> logger
        )
    {
        var sw = Stopwatch.StartNew();

        await _next(httpContext);

        sw.Stop();

        logger.LogInformation(
            "{Method} {Path} responded {StatusCode} in {ElapsedMs}ms", 
            httpContext.Request.Method, 
            httpContext.Request.Path, 
            httpContext.Response.StatusCode, 
            sw.ElapsedMilliseconds
            );
    }
}