using System.Threading.Channels;

namespace TaskFlow.Application.BackgroundJobs;

public class NotificationQueue : INotificationQueue 
{
    private Channel<INotification> _channel;

    public NotificationQueue()
    {
        _channel = Channel.CreateBounded<INotification>(
            new BoundedChannelOptions(capacity: 1000)
            {
                FullMode     = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            });
    }
    public async ValueTask EnqueueAsync(INotification notification, CancellationToken ct)
        => await _channel.Writer.WriteAsync(notification, ct);
    
    public IAsyncEnumerable<INotification> ReadAllAsync(CancellationToken ct)
        => _channel.Reader.ReadAllAsync(ct);
}