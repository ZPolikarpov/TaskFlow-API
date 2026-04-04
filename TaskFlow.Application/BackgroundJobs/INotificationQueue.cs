namespace TaskFlow.Application.BackgroundJobs;

public interface INotificationQueue 
{
    ValueTask EnqueueAsync(INotification notification, CancellationToken ct);
    IAsyncEnumerable<INotification> ReadAllAsync(CancellationToken ct);
}