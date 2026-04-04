using Microsoft.Extensions.Diagnostics.HealthChecks;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Api.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly AppDbContext _db;
    public DatabaseHealthCheck(
        AppDbContext db
    )
    {
        _db = db;
    }
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext healthCheckContext, CancellationToken ct)
    {
        try
        {
            var canConnect = await _db.Database.CanConnectAsync(ct);

            return canConnect
                ? HealthCheckResult.Healthy("Database is reachable")
                : HealthCheckResult.Unhealthy("Cannot connect to database");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Database health check threw an exception",
                exception: ex);
        }
    }
}