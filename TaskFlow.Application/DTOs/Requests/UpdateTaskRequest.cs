using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.DTOs.Requests;

public record UpdateTaskRequest(
    string? Title, 
    AppTaskPriority? Priority,
    string? Description,
    DateTimeOffset? DueDate,
    AppTaskStatus? Status);