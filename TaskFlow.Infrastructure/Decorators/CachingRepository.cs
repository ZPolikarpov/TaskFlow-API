using Microsoft.Extensions.Caching.Memory;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Infrastructure.Decorators;

public class CachingRepository<T> : IRepository<T> where T : class, IEntity
{
    private readonly IMemoryCache _cache;
    private readonly IRepository<T> _inner;
    public CachingRepository(IMemoryCache cache, IRepository<T> inner){
        _cache = cache;
        _inner = inner;
    }
    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        _cache.TryGetValue($"{typeof(T).Name}:{id}", out T? result);
        if (result is not null) 
            return result;
        result = await _inner.GetByIdAsync(id, ct);
        _cache.Set($"{typeof(T).Name}:{id}", result, TimeSpan.FromMinutes(5));
        return result;
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
    {
        var result = await _inner.GetAllAsync(ct);
        return result;
    }

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        var result = await _inner.AddAsync(entity, ct);
        return result;
    }

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        await _inner.UpdateAsync(entity, ct);
        _cache.Remove($"{typeof(T).Name}:{entity.Id}");
        _cache.Set($"{typeof(T).Name}:{entity.Id}", entity, TimeSpan.FromMinutes(5));
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await _inner.DeleteAsync(id, ct);
        _cache.Remove($"{typeof(T).Name}:{id}");
    }
}