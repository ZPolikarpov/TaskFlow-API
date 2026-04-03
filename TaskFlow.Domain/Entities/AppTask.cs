namespace TaskFlow.Domain.Entities;

/// <summary>
/// Represents a task within the system, including its metadata such as title, status,
/// priority, ownership, assignment, and lifecycle timestamps.
/// </summary>
public class AppTask : IEntity
{
    public int Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; } = string.Empty;
    public AppTaskStatus Status { get; private set; }
    public AppTaskPriority Priority { get; private set; }
    public int ProjectId { get; private set; }
    public int OwnerId { get; private set; }
    public int? AssigneeId { get; private set; }
    public DateTimeOffset? DueDate { get; private set; }
    public DateTimeOffset CreatedOn { get; private set; }
    public DateTimeOffset UpdatedOn { get; private set; }

    public Project? Project { get; private set; }
    public User? Owner { get; private set; }
    public User? Assignee { get; private set; }

    /// <summary>
    /// Creates a new instance of <see cref="AppTask"/> with the specified parameters.
    /// </summary>
    /// <param name="title">The title of the task. Cannot be null or empty.</param>
    /// <param name="priority">The priority level of the task.</param>
    /// <param name="projectId">The identifier of the project the task belongs to.</param>
    /// <param name="ownerId">The identifier of the user creating the task.</param>
    /// <param name="description">Optional description of the task.</param>
    /// <param name="AssigneeId">Optional identifier of the assigned user.</param>
    /// <param name="status">Initial status of the task. Defaults to Todo.</param>
    /// <param name="dueDate">Optional due date for the task.</param>
    /// <returns>A newly created <see cref="AppTask"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the title is null or empty.</exception>
    public static AppTask Create(
        string title, AppTaskPriority priority,
        int projectId, int ownerId,
        string? description = null, int? AssigneeId = null, 
        AppTaskStatus status = AppTaskStatus.Todo, DateTimeOffset? dueDate = null
    )
    {
        ArgumentException.ThrowIfNullOrEmpty(title);

        return new AppTask()
        {
            Title = title.Trim(),
            Description = description?.Trim(),
            Status = status,
            Priority = priority,
            ProjectId = projectId,
            OwnerId = ownerId,
            AssigneeId = AssigneeId,
            DueDate = dueDate,
            CreatedOn = DateTimeOffset.UtcNow,
            UpdatedOn = DateTimeOffset.UtcNow
        };
    }

    /// <summary>
    /// Assigns the task to a specific user.
    /// </summary>
    /// <param name="assigneeId">The identifier of the user to assign the task to.</param>
    public void Assign(int assigneeId)
    {
        AssigneeId = assigneeId;
        UpdatedOn = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Removes any existing assignment from the task.
    /// </summary>
    public void Unassign()
    {
        AssigneeId = null;
        UpdatedOn = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Updates the status of the task.
    /// </summary>
    /// <param name="status">The new status to apply.</param>
    public void SetStatus(AppTaskStatus status)
    {
        Status = status;
        UpdatedOn = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Updates the priority level of the task.
    /// </summary>
    /// <param name="priority">The new priority to apply.</param>
    public void SetPriority(AppTaskPriority priority)
    {
        Priority = priority;
        UpdatedOn = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Updates the description of the task.
    /// </summary>
    /// <param name="description">The new description, or null to clear it.</param>
    public void SetDescription(string? description)
    {
        Description = description?.Trim();
        UpdatedOn = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Sets or updates the due date of the task.
    /// </summary>
    /// <param name="dueDate">The new due date, or null to remove it.</param>
    public void SetDueDate(DateTimeOffset? dueDate)
    {
        DueDate = dueDate;
        UpdatedOn = DateTimeOffset.UtcNow;
    }
}