namespace TaskFlow.Application.BackgroundJobs;

public interface INotificationHandler<TNotification> where TNotification : INotification
{
    Task HandleAsync(TNotification notification, CancellationToken ct);
}