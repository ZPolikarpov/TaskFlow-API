using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using TaskFlow.Api.HealthChecks;
using TaskFlow.Api.Infrastructure;
using TaskFlow.Api.Middleware;
using TaskFlow.Application.Validators;
using TaskFlow.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();

// FluentValidation
services.AddFluentValidationAutoValidation();
services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

// EF Core — SQL Server
services.AddInfrastructure(builder.Configuration);

services.AddApiVersioning(opts =>
{
    opts.DefaultApiVersion = new ApiVersion(1);
    opts.AssumeDefaultVersionWhenUnspecified = true;
    opts.ReportApiVersions = true;
}).AddApiExplorer(opts =>
{
    opts.GroupNameFormat = "'v'VVV";
    opts.SubstituteApiVersionInUrl = true;
});

// Swagger
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

// Error handling
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestTimingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI(opts =>
    {
        foreach (var desc in app.DescribeApiVersions())
            opts.SwaggerEndpoint(
                $"/swagger/{desc.GroupName}/swagger.json",
                desc.GroupName);

        opts.DefaultModelsExpandDepth(-1);
        opts.DisplayRequestDuration();
    });
}

app.Run();