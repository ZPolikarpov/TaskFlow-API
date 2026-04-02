using TaskFlow.Domain.Entities;
namespace TaskFlow.Domain.Repositories;

public interface ITaskRepository : IRepository<Project>
{
    Task<IEnumerable<Project>> GetByWorkspaceAsync(int workspaceId);  
    Task<bool> ExistsAsync(int projectId, int workspaceId); 
}