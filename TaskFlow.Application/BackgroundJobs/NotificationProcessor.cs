using Microsoft.Extensions.DependencyInjection;  
using Microsoft.Extensions.Hosting;             
using Microsoft.Extensions.Logging; 

namespace TaskFlow.Application.BackgroundJobs;

public class NotificationProcessor : BackgroundService
{
    private readonly INotificationQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NotificationProcessor> _logger;

    public NotificationProcessor(
        INotificationQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<NotificationProcessor> logger)
    {
        _queue       = queue;
        _scopeFactory = scopeFactory;
        _logger      = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _logger.LogInformation("Notification processor started");

        await foreach (var notification in _queue.ReadAllAsync(ct))
        {
            await ProcessNotificationAsync(notification, ct);
        }

        _logger.LogInformation("Notification processor stopped");
    }

    private async Task ProcessNotificationAsync(
        INotification notification, CancellationToken ct)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();

            var handlerType = typeof(INotificationHandler<>)
                .MakeGenericType(notification.GetType());

            var handler = scope.ServiceProvider.GetService(handlerType);

            if (handler is null)
            {
                _logger.LogWarning(
                    "No handler registered for notification type {Type}",
                    notification.GetType().Name);
                return;
            }

            var handleMethod = handlerType.GetMethod("HandleAsync")!;
            await (Task)handleMethod.Invoke(handler,
                [ notification, ct ])!;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to process notification of type {Type}",
                notification.GetType().Name);
        }
    }
}