namespace TaskFlow.Domain.Entities;
public enum AppTaskStatus
{
    Todo = 0,
    InProgress = 1,
    InReview = 2,
    Done = 3
}
public enum AppTaskPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}