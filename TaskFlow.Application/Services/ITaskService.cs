using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Requests;
using TaskFlow.Application.DTOs.Responses;

namespace TaskFlow.Application.Services;

public interface ITaskService
{
    Task<Result<TaskResponse>> CreateAsync(CreateTaskRequest req, CancellationToken ct = default);
    Task<Result<TaskResponse>> GetByIdAsync(int taskId, CancellationToken ct = default);
    Task<Result<TaskResponse>> UpdateAsync(int taskid, UpdateTaskRequest req, CancellationToken ct = default);
    Task<Result<TaskResponse>> AssignAsync(int taskId, AssignTaskRequest req, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int taskId, CancellationToken ct = default);
    Task<Result<PagedResponse<TaskResponse>>> GetPagedAsync(int projectId, int page, int pageSize, CancellationToken ct = default);
}