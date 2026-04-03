using TaskFlow.Domain.Entities;
namespace TaskFlow.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> FindByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync( 
        string email, CancellationToken ct = default);
}