using TaskFlow.Domain.Entities;
namespace TaskFlow.Domain.Repositories;

public interface IProjectRepository : IRepository<Project>
{
    Task<IEnumerable<Project>> GetByWorkspaceAsync(int workspaceId, CancellationToken ct = default);  
    Task<bool> ExistsInWorkspaceAsync(int projectId, int workspaceId, CancellationToken ct = default);
}