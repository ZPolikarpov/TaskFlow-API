using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Infrastructure.Persistence;

public class EfRepository<T> : IRepository<T> where T : class
{
    private readonly DbContext _AppDbContext;

    public EfRepository(DbContext appDbContext){
        _AppDbContext = appDbContext;
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _AppDbContext.Set<T>().FindAsync([id], ct);
    }
    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
    {
        return await _AppDbContext.Set<T>().ToListAsync(ct);
    }
    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await _AppDbContext.Set<T>().AddAsync(entity, ct);
        await _AppDbContext.SaveChangesAsync(ct);
        return entity;
    }
    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _AppDbContext.Set<T>().Update(entity);
        await _AppDbContext.SaveChangesAsync(ct);
    }
    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _AppDbContext.Set<T>().FindAsync([id], ct);
        if (entity is null) return;
        
        _AppDbContext.Set<T>().Remove(entity);
        await _AppDbContext.SaveChangesAsync(ct);
    }
}