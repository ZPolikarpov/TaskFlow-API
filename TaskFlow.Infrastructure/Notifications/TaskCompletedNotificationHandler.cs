using Microsoft.Extensions.Logging;
using TaskFlow.Application.BackgroundJobs;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Infrastructure.Notifications;

public class TaskCompletedNotificationHandler : INotificationHandler<TaskCompletedNotification>
{
    private readonly IUserRepository _users;
    private readonly ILogger<TaskCompletedNotificationHandler> _logger;

    public TaskCompletedNotificationHandler(
        IUserRepository users,
        ILogger<TaskCompletedNotificationHandler> logger
    )
    {
        _users = users;
        _logger = logger;
    }
    public async Task HandleAsync(TaskCompletedNotification notification, CancellationToken ct)
    {       
        var user = await _users.GetByIdAsync(notification.OwnerId, ct);

        if (user is null)
        {
            _logger.LogWarning("Couldn't find a user with the id {userId} to send notification mail.", notification.OwnerId);
            return;
        }

        _logger.LogInformation("Notifying user {Email} that task '{Title}' was completed.", user!.Email, notification.Title);
    }
}