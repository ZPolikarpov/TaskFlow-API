using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Tests.Infrastructure;

public class TaskFlowWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection =
        new SqliteConnection("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove all SQL Server DbContext registrations
            var descriptorsToRemove = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>)  ||
                    d.ServiceType == typeof(DbContextOptions)                ||
                    d.ServiceType == typeof(AppDbContext)                    ||
                    d.ServiceType == typeof(
                        IDbContextOptionsConfiguration<AppDbContext>))
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
                services.Remove(descriptor);

            // Use the shared connection — all contexts share the same
            // in-memory database because they share the same connection
            services.AddDbContext<AppDbContext>(opts =>
                opts.UseSqlite(_connection));

            // Create schema using the shared connection
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();
                
            db.Database.OpenConnection();
            db.Database.EnsureCreated();
        });
    }
}