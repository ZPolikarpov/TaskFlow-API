using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.DTOs.Responses;

public record TaskResponse(
    int Id,
    int ProjectId,
    string Title,
    string Status,
    string Priority,
    int OwnerId,
    DateTimeOffset CreatedOn,
    DateTimeOffset UpdatedOn,
    DateTimeOffset? DueDate,
    string? Description,
    int? AssigneeId,
    string? AssigneeDisplayName
    )
{
    public static TaskResponse From(AppTask task) => new(
        task.Id,
        task.ProjectId,
        task.Title,
        task.Status.ToString(),
        task.Priority.ToString(),
        task.OwnerId,
        task.CreatedOn,
        task.UpdatedOn,
        task.DueDate,
        task.Description,
        task.AssigneeId,
        task.Assignee?.DisplayName
        );
}