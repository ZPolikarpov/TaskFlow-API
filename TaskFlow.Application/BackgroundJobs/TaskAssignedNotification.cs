namespace TaskFlow.Application.BackgroundJobs;

public record TaskAssignedNotification(
    int TaskId,
    string Title,
    int AssigneeId
) : INotification;
