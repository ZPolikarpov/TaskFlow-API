using Microsoft.Extensions.Logging;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Infrastructure.Decorators;

public class LoggingRepository<T> : IRepository<T> where T : class, IEntity
{
    private readonly ILogger<LoggingRepository<T>> _logger;
    private readonly IRepository<T> _inner;
    public LoggingRepository(ILogger<LoggingRepository<T>> logger, IRepository<T> inner){
        _logger = logger;
        _inner = inner;
    }
    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        _logger.LogInformation("[{Entity}] GetByIdAsync({Id})", typeof(T).Name, id);
        var result = await _inner.GetByIdAsync(id, ct);
        _logger.LogInformation("[{Entity}] GetByIdAsync({Id}) → {Found}",
            typeof(T).Name, id, result is null ? "not found" : "found");
        return result;
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("[{Entity}] GetAllAsync()", typeof(T).Name);
        var result = await _inner.GetAllAsync(ct);
        return result;
    }

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        _logger.LogInformation("[{Entity}] AddAsync()", typeof(T).Name);
        var result = await _inner.AddAsync(entity, ct);
        _logger.LogInformation("[{Entity}] AddAsync() → success", typeof(T).Name);
        return result;
    }

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _logger.LogInformation("[{Entity}] UpdateAsync()", typeof(T).Name);
        await _inner.UpdateAsync(entity, ct);
        _logger.LogInformation("[{Entity}] UpdateAsync() → success", typeof(T).Name);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        _logger.LogInformation("[{Entity}] DeleteAsync({Id})", typeof(T).Name, id);
        await _inner.DeleteAsync(id, ct);
        _logger.LogInformation("[{Entity}] DeleteAsync({Id}) → success",
            typeof(T).Name, id);
    }
}