using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.DTOs.Responses;

public record ProjectResponse(
    int Id,
    int WorkspaceId,
    string Name,
    string? Description,
    int OwnerId,
    string OwnerDisplayName,
    bool IsArchived,
    DateTimeOffset CreatedOn,
    DateTimeOffset UpdatedOn
    )
{
    public static ProjectResponse From(Project project) => new(
        project.Id,
        project.WorkspaceId,
        project.Name,
        project.Description,
        project.OwnerId,
        project.Owner?.DisplayName ?? string.Empty,
        project.IsArchived,
        project.CreatedOn,
        project.UpdatedOn);
}