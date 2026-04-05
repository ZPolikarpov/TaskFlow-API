namespace TaskFlow.Domain.Entities;

/// <summary>
/// Represents a project within a workspace, used to group and organize related tasks.
/// </summary>
public class Project : IEntity
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; } = string.Empty;
    public int WorkspaceId { get; private set; }
    public int OwnerId { get; private set; }
    public bool IsArchived { get; private set; } = false;
    public DateTime CreatedOn { get; private set; }
    public DateTime UpdatedOn { get; private set; }
    public Workspace? Workspace { get; private set; }
    public User? Owner { get; private set; }
    public ICollection<AppTask> Tasks { get; private set; } = new List<AppTask>();

    /// <summary>
    /// Creates a new <see cref="Project"/> instance with the specified parameters.
    /// </summary>
    /// <param name="name">The name of the project. Cannot be null or empty.</param>
    /// <param name="workspaceId">The identifier of the workspace the project belongs to.</param>
    /// <param name="ownerId">The identifier of the user who owns the project.</param>
    /// <param name="description">Optional description providing additional details about the project.</param>
    /// <returns>A newly created <see cref="Project"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the name is null or empty.</exception>
    public static Project Create(
        string name, int workspaceId, 
        int ownerId, string? description = null
    )
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        return new Project()
        {
            Name = name.Trim(),
            Description = description?.Trim(),
            WorkspaceId = workspaceId,
            OwnerId = ownerId,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Archives the project, marking it as inactive.
    /// </summary>
    public void Archive()
    {
        IsArchived = true;
        UpdatedOn = DateTime.UtcNow;
    } 

    /// <summary>
    /// Renames the project.
    /// </summary>
    /// <param name="newName">The new name of the project. Cannot be null or empty.</param>
    /// <exception cref="ArgumentException">Thrown when the new name is null or empty.</exception>
    public void Rename(string newName)
    {
        ArgumentException.ThrowIfNullOrEmpty(newName);

        Name = newName;
        UpdatedOn = DateTime.UtcNow;
    }
}