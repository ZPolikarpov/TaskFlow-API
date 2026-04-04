using TaskFlow.Domain.Entities;
namespace TaskFlow.Domain.Repositories;

public interface ITaskRepository : IRepository<AppTask>
{
    Task<IEnumerable<AppTask>> GetByProjectAsync(int projectId, CancellationToken ct = default);  
    Task<IEnumerable<AppTask>> GetByAssigneeAsync(int userId, CancellationToken ct = default);  
    Task<(IEnumerable<AppTask> Items, int TotalCount)> GetPagedAsync(
        int projectId, int page, int pageSize,
        CancellationToken ct = default);  
}