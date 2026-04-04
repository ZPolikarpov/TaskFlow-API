namespace TaskFlow.Application.BackgroundJobs;

public record TaskCompletedNotification(
    int TaskId,
    string Title,
    int OwnerId
) : INotification;
