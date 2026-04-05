using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.DTOs.Requests;

public record CreateTaskRequest(
    string Title, 
    AppTaskPriority Priority,
    string? Description,
    int? AssigneeId,
    DateTime? DueDate,
    AppTaskStatus? Status);

public record CreateTaskCommand(
    string Title, 
    AppTaskPriority Priority,
    int ProjectId, 
    string? Description,
    int? AssigneeId,
    DateTime? DueDate,
    AppTaskStatus? Status);