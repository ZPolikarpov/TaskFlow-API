using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Requests;
using TaskFlow.Application.DTOs.Responses;

namespace TaskFlow.Application.Services;

public interface IProjectService
{
    Task<Result<ProjectResponse>> CreateAsync(CreateProjectRequest req, CancellationToken ct = default);
    Task<Result<ProjectResponse>> GetByIdAsync(int projectId, CancellationToken ct = default);
    Task<Result<IEnumerable<ProjectResponse>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<ProjectResponse>> ArchiveAsync(int projectId, CancellationToken ct = default);
}