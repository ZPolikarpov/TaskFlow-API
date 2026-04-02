namespace TaskFlow.Domain.Entities;
public class Project
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; } = string.Empty;
    public int WorkspaceId { get; private set; }
    public int OwnerId { get; private set; }
    public bool IsArchived { get; private set; } = false;
    public DateTimeOffset CreatedOn { get; private set; }
    public DateTimeOffset UpdatedOn { get; private set; }

    
    public static Project Create(
        string name, string? description,
        int workspaceId, int ownerId
    )
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        return new Project()
        {
            Name = name.Trim(),
            Description = description.Trim(),
            WorkspaceId = workspaceId,
            OwnerId = ownerId,
            CreatedOn = DateTimeOffset.UtcNow,
            UpdatedOn = DateTimeOffset.UtcNow
        };
    }
}