namespace TaskFlow.Domain.Entities;

public class AppTask
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

    public static AppTask Create(
        string title, string? description,
        AppTaskStatus status, AppTaskPriority priority,
        int projectId, int ownerId,
        int? AssigneeId, DateTimeOffset? dueDate
    )
    {
        ArgumentException.ThrowIfNullOrEmpty(title);

        return new AppTask()
        {
            Title = title.Trim(),
            Description = description.Trim(),
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
    
}