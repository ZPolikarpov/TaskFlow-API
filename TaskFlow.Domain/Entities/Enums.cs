namespace TaskFlow.Domain.Entities;

/// <summary>
/// Represents the lifecycle state of a task within the system.
/// </summary>
public enum AppTaskStatus
{
    Todo = 0,
    InProgress = 1,
    InReview = 2,
    Done = 3
}

/// <summary>
/// Represents the urgency and importance level of a task.
/// </summary>
public enum AppTaskPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}