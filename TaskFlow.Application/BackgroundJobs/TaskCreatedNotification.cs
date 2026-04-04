namespace TaskFlow.Application.BackgroundJobs;

public record TaskCreatedNotification(
    int TaskId,
    string Title,
    int ProjectId,
    int? AssigneeId
) : INotification;
