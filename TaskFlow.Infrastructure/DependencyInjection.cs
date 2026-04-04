using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using TaskFlow.Application.BackgroundJobs;
using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Services;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Repositories;
using TaskFlow.Infrastructure.Auth;
using TaskFlow.Infrastructure.Decorators;
using TaskFlow.Infrastructure.Notifications;
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
        AddDecoratedRepository<User>     (services);
        AddDecoratedRepository<Workspace>(services);
        AddDecoratedRepository<Project>  (services);
        AddDecoratedRepository<AppTask>  (services);

        // Domain-specific repositories
        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IProjectRepository, EfProjectRepository>();
        services.AddScoped<ITaskRepository, EfTaskRepository>();
       
        // Auth services
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Application services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITaskService, TaskService>();

        // Notification services
        services.AddSingleton<INotificationQueue, NotificationQueue>();
        services.AddHostedService<NotificationProcessor>();
        services.AddScoped<INotificationHandler<TaskCreatedNotification>,
            TaskCreatedNotificationHandler>();
        services.AddScoped<INotificationHandler<TaskCompletedNotification>,
            TaskCompletedNotificationHandler>();
        services.AddScoped<INotificationHandler<TaskAssignedNotification>,
            TaskAssignedNotificationHandler>();

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
                opts.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = ctx =>
                    {
                        var logger = ctx.HttpContext.RequestServices
                            .GetRequiredService<ILogger<JwtBearerEvents>>();
                        logger.LogWarning(
                            "JWT authentication failed: {Error}",
                            ctx.Exception.Message);
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
    private static IServiceCollection AddDecoratedRepository<TEntity>(
        IServiceCollection services)
        where TEntity : class, IEntity
    {
        // Register concrete EF implementation so the factory can resolve it
        services.AddScoped<EfRepository<TEntity>>();

        // Build the decorator chain manually but generically
        services.AddScoped<IRepository<TEntity>>(sp =>
        {
            var inner  = sp.GetRequiredService<EfRepository<TEntity>>();
            var logger = sp.GetRequiredService<ILogger<LoggingRepository<TEntity>>>();
            var cache  = sp.GetRequiredService<IMemoryCache>();

            return new CachingRepository<TEntity>(
                cache,
                new LoggingRepository<TEntity>(logger, inner)
                );
        });

        return services;
    }
}