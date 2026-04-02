namespace TaskFlow.Domain.Entities;
public class Enums
{
    public readonly enum AppTaskStatus
    {
        Todo = 0,
        InProgress = 1,
        InReview = 2,
        Done = 3
    }
    public readonly enum AppTaskPriority
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Critical = 3
    }
}