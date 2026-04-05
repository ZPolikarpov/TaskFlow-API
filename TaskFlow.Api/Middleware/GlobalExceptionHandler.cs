using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Domain.Exceptions;

namespace TaskFlow.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken ct
        )
    {
        var (status, title, errors) = exception switch
        {
            NotFoundException e    => (404, e.Message, null),
            ConflictException e    => (409, e.Message, null),
            ForbiddenException e   => (403, e.Message, null),
            ValidationException e  => (422, "One or more validation errors occurred.",
                e.Errors
                 .GroupBy(f => f.PropertyName)
                 .ToDictionary(
                     g => g.Key,
                     g => g.Select(f => f.ErrorMessage).ToArray())),
            _                      => (500, "An unexpected error occurred.", null)
        };

        if (status == 500)
            _logger.LogError(exception,
                "Unhandled exception for {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path);
        else
            _logger.LogWarning(exception,
                "Domain exception {Status} for {Method} {Path}",
                status,
                httpContext.Request.Method,
                httpContext.Request.Path);

        httpContext.Response.StatusCode = status;

        if (errors is not null)
        {
            var validationProblem = new ValidationProblemDetails(errors)
            {
                Status   = status,
                Title    = title,
                Instance = httpContext.Request.Path
            };
            await httpContext.Response.WriteAsJsonAsync(validationProblem, ct);
        }
        else
        {
            var problem = new ProblemDetails
            {
                Status   = status,
                Title    = title,
                Instance = httpContext.Request.Path,
                Detail   = status == 500 ? null : exception.Message
            };
            await httpContext.Response.WriteAsJsonAsync(problem, ct);
        }

        return true;
    }
}