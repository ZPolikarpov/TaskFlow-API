using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Infrastructure.Persistence;

public class EfProjectRepository : EfRepository<Project>, IProjectRepository 
{
    public EfProjectRepository(AppDbContext AppDbContext) : base(AppDbContext) { }

    public async Task<IEnumerable<Project>> GetByWorkspaceAsync(int workspaceId, CancellationToken ct = default)
        => await _AppDbContext.Projects
                    .Where(p => p.WorkspaceId == workspaceId)
                    .OrderByDescending(p => p.CreatedOn)
                    .ToListAsync(ct);
    public async Task<bool> ExistsInWorkspaceAsync(int projectId, int workspaceId, CancellationToken ct = default)
        => await _AppDbContext.Projects.AnyAsync(p => p.Id == projectId && p.WorkspaceId == workspaceId, ct);
}