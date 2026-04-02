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
        string title, AppTaskPriority priority,
        int projectId, int ownerId,
        string? description = string.Empty, int? AssigneeId = null, 
        AppTaskStatus? status = AppTaskStatus.Todo, DateTimeOffset? dueDate = null
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

    public void Assign(int assigneeId)
    {
        AssigneeId = assigneeId;
        UpdatedOn = DateTimeOffset.UtcNow;
    }
    public void Unassign()
    {
        AssigneeId = null;
        UpdatedOn = DateTimeOffset.UtcNow;
    }
    public void SetStatus(AppTaskStatus status)
    {
        Status = status;
        UpdatedOn = DateTimeOffset.UtcNow;
    }
    public void SetPriority(AppTaskPriority priority)
    {
        Priority = priority;
        UpdatedOn = DateTimeOffset.UtcNow;
    }
    public void SetDescription(string? description)
    {
        Description = description?.Trim();
        UpdatedOn = DateTimeOffset.UtcNow;
    }
    public void SetDueDate(DateTimeOffset? dueDate)
    {
        DueDate = dueDate;
        UpdatedOn = DateTimeOffset.UtcNow;
    }
}