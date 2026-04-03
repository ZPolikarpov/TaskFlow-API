using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Services;
using TaskFlow.Domain.Repositories;
using TaskFlow.Infrastructure.Auth;
using TaskFlow.Infrastructure.Decorators;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseSqlServer(
                configuration.GetConnectionString("Default"),
                sql => sql.MigrationsAssembly("TaskFlow.Infrastructure")));

        services.AddMemoryCache();
        services.AddHttpContextAccessor();

        // Generic repository with decorator stack
        services.AddScoped(typeof(EfRepository<>));
        services.AddScoped(typeof(IRepository<>),
            typeof(EfRepository<>));
        services.Decorate(typeof(IRepository<>),
            typeof(LoggingRepository<>));
        services.Decorate(typeof(IRepository<>),
            typeof(CachingRepository<>));

        // Domain-specific repositories
        services.AddScoped<IUserRepository, EfUserRepository>();
       
        // Auth services
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IAuthService, AuthService>();

        // JWT authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)),
                    ValidateIssuer   = true,
                    ValidIssuer      = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience    = configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew        = TimeSpan.FromSeconds(30)
                };
            });

        services.AddAuthorization();

        return services;
    }
}