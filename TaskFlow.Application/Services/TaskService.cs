using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Requests;
using TaskFlow.Application.DTOs.Responses;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _tasks;
    private readonly IProjectRepository _projects;
    private readonly ICurrentUserService _currentUser;
    public TaskService(
        ITaskRepository tasks,
        IProjectRepository projects,
        ICurrentUserService currentUser
    )
    {
        _tasks = tasks;
        _projects = projects;
        _currentUser = currentUser;
    }
    public async Task<Result<TaskResponse>> CreateAsync(CreateTaskRequest req, CancellationToken ct = default)
    {
        bool projectExists = await _projects.ExistsInWorkspaceAsync(req.ProjectId, _currentUser.WorkspaceId, ct);
        if (!projectExists)
            return Result<TaskResponse>.NotFound($"Project with id {req.ProjectId} could not be found!");

        var task = AppTask.Create(
            req.Title,
            req.Priority,
            req.ProjectId,
            _currentUser.UserId,
            req.Status,
            req.Description,
            req.AssigneeId,
            req.DueDate
        );

        await _tasks.AddAsync(task, ct);

        return Result<TaskResponse>.Success(TaskResponse.From(task));
    }
    public async Task<Result<TaskResponse>> GetByIdAsync(int taskId, CancellationToken ct = default)
    {
        var task = await _tasks.GetByIdAsync(taskId, ct);
        if (task is null || task?.Project?.WorkspaceId != _currentUser.WorkspaceId)
            return Result<TaskResponse>.NotFound($"Task with id {taskId} could not be found!");

        return Result<TaskResponse>.Success(TaskResponse.From(task)); 
    }
    public async Task<Result<TaskResponse>> AssignAsync(int taskId, AssignTaskRequest req, CancellationToken ct = default)
    {
        var task = await _tasks.GetByIdAsync(taskId, ct);
        if (task is null || task?.Project?.WorkspaceId != _currentUser.WorkspaceId)
            return Result<TaskResponse>.NotFound($"Task with id {taskId} could not be found!");

        task.Assign(req.AssigneeId);

        return Result<TaskResponse>.Success(TaskResponse.From(task)); 
    }
    public async Task<Result<bool>> DeleteAsync(int taskId, CancellationToken ct = default)
    {
        var task = await _tasks.GetByIdAsync(taskId, ct);
        if (task is null)
            return Result<bool>.NotFound($"Task with id {taskId} could not be found!");

        if (task.OwnerId != _currentUser.UserId)
            return Result<bool>.Unauthorized($"Only the owner can delete this task!"); 

        return Result<bool>.Success(true); 
    }
    public async Task<Result<TaskResponse>> UpdateAsync(
        int taskId, UpdateTaskRequest request,
        CancellationToken ct = default)
    {
        var task = await _tasks.GetByIdAsync(taskId, ct);

        if (task is null || task?.Project?.WorkspaceId != _currentUser.WorkspaceId)
            return Result<TaskResponse>.NotFound(
                $"Task {taskId} was not found");

        if (task.OwnerId != _currentUser.UserId)
            return Result<TaskResponse>.Forbidden(
                "Only the task owner can update a task");

        if (request.Title is not null)
            task.SetTitle(request.Title);

        if (request.Description is not null)
            task.SetDescription(request.Description);

        if (request.Status.HasValue)
            task.SetStatus(request.Status.Value);

        if (request.Priority.HasValue)
            task.SetPriority(request.Priority.Value);

        if (request.DueDate.HasValue)
            task.SetDueDate(request.DueDate);

        await _tasks.UpdateAsync(task, ct);

        return Result<TaskResponse>.Success(TaskResponse.From(task));
    }
    public async Task<Result<PagedResponse<TaskResponse>>> GetPagedAsync(int projectId, int page, int pageSize, CancellationToken ct = default)
    {
        bool projectExists = await _projects.ExistsInWorkspaceAsync(projectId, _currentUser.WorkspaceId, ct);
        if (!projectExists)
            return Result<PagedResponse<TaskResponse>>.NotFound($"Project with id {projectId} could not be found!");

        (var tasks, int totalCount) = await _tasks.GetPagedAsync(
            projectId,
            page,
            pageSize,
            ct
        );
        
        return Result<PagedResponse<TaskResponse>>.Success(new PagedResponse<TaskResponse>{
                Items      = tasks.Select(TaskResponse.From),
                TotalCount = totalCount,
                Page       = page,
                PageSize   = pageSize
            }
        );
    }
}