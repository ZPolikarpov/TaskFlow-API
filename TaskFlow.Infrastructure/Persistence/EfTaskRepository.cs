using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Infrastructure.Persistence;

public class EfTaskRepository : EfRepository<AppTask>, ITaskRepository 
{
    public EfTaskRepository(AppDbContext AppDbContext) : base(AppDbContext) { }

    public async Task<IEnumerable<AppTask>> GetByProjectAsync(int projectId, CancellationToken ct = default)
        => await _AppDbContext.Tasks
                    .Where(t => t.ProjectId == projectId)
                    .OrderByDescending(t => t.CreatedOn)
                    .ToListAsync(ct);
    public async Task<IEnumerable<AppTask>> GetByAssigneeAsync(int userId, CancellationToken ct = default)
        => await _AppDbContext.Tasks.Where(t => t.AssigneeId == userId)
                    .OrderByDescending(t => t.CreatedOn)
                    .ToListAsync(ct);
    public async Task<(IEnumerable<AppTask> Items, int TotalCount)> GetPagedAsync(
        int projectId, int page, int pageSize,
        CancellationToken ct = default)
    {
        int totalCount = await _AppDbContext.Tasks.Where(t => t.ProjectId == projectId).CountAsync(ct);
        var tasks = await _AppDbContext.Tasks
                            .Where(t => t.ProjectId == projectId)
                            .OrderByDescending(t => t.CreatedOn)
                            .Skip((page-1)*pageSize)
                            .Take(pageSize)
                            .ToListAsync(ct);
        
        return (tasks, totalCount);
    }

    public override async Task<AppTask?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _AppDbContext.Tasks
            .Include(t => t.Project)
                .ThenInclude(p => p!.Workspace)
            .FirstOrDefaultAsync(t => t.Id == id, ct);
    }
}