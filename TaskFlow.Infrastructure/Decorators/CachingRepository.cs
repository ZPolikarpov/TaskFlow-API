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
    private static string GenerateCacheKey(int id) => $"{typeof(T).Name}:{id}";
    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var key = GenerateCacheKey(id);

        if(_cache.TryGetValue(key, out T? cached))
            return cached;

        var result = await _inner.GetByIdAsync(id, ct);

        if (result is not null)
            _cache.Set(key, result, TimeSpan.FromMinutes(5));

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
        var key = GenerateCacheKey(entity.Id);

        await _inner.UpdateAsync(entity, ct);
        _cache.Remove(key);
        _cache.Set(key, entity, TimeSpan.FromMinutes(5));
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var key = GenerateCacheKey(id);

        await _inner.DeleteAsync(id, ct);
        _cache.Remove(key);
    }
}