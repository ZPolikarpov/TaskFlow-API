namespace TaskFlow.Application.BackgroundJobs;

public interface INotificationQueue 
{
    ValueTask EnqueueAsync(INotification notification, CancellationToken ct);
    Task<IEnumerable<INotification>> ReadAllAsync(CancellationToken ct);
}