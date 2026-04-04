using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TaskFlow.Api.Infrastructure;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider  _apiVersionProvider;
    public ConfigureSwaggerOptions(
        IApiVersionDescriptionProvider  apiVersionProvider
    )
    {
        _apiVersionProvider = apiVersionProvider;
    }
    public void Configure(SwaggerGenOptions genOptions)
    {
        foreach (var description in _apiVersionProvider.ApiVersionDescriptions)
        {
            genOptions.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title   = $"TaskFlow API {description.GroupName}",
                Version = description.GroupName,
                Description = description.IsDeprecated
                    ? "This API version has been deprecated."
                    : "Task management REST API"
            });
        }

        // JWT bearer authentication in Swagger UI
        genOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name         = "Authorization",
            Type         = SecuritySchemeType.Http,
            Scheme       = "bearer",
            BearerFormat = "JWT",
            In           = ParameterLocation.Header,
            Description  = "Enter your JWT token. Example: eyJhbG..."
        });

        genOptions.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id   = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    }
}