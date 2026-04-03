using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Domain.Repositories;
using TaskFlow.Infrastructure.Decorators;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseSqlServer(
                configuration.GetConnectionString("Default"),
                sql => sql.MigrationsAssembly("TaskFlow.Infrastructure")));

        services.AddMemoryCache();

        services.AddScoped(typeof(IRepository<>),
            typeof(EfRepository<>));

        services.Decorate(typeof(IRepository<>),
            typeof(LoggingRepository<>));

        services.Decorate(typeof(IRepository<>),
            typeof(CachingRepository<>));

        return services;
    }
}