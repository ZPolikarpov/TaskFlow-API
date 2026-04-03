using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Infrastructure.Persistence;

public class EfUserRepository : EfRepository<User>, IUserRepository 
{
    public EfUserRepository(AppDbContext AppDbContext) : base(AppDbContext) { }

    public async Task<User?> FindByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _AppDbContext.Users.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);
    }

    public async Task<bool> ExistsByEmailAsync( 
        string email, CancellationToken ct = default)
        => await _AppDbContext.Users
            .AnyAsync(u => u.Email == email.ToLowerInvariant(), ct);
}