using Microsoft.Extensions.Logging;
using TaskFlow.Application.BackgroundJobs;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Infrastructure.Notifications;

public class TaskAssignedNotificationHandler : INotificationHandler<TaskAssignedNotification>
{
    private readonly IUserRepository _users;
    private readonly ILogger<TaskAssignedNotificationHandler> _logger;

    public TaskAssignedNotificationHandler(
        IUserRepository users,
        ILogger<TaskAssignedNotificationHandler> logger
    )
    {
        _users = users;
        _logger = logger;
    }
    public async Task HandleAsync(TaskAssignedNotification notification, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(notification.AssigneeId, ct);

        if (user is null)
        {
            _logger.LogWarning("Couldn't find a user with the id {userId} to send notification mail.", notification.AssigneeId);
            return;
        }

        _logger.LogInformation("Notifying user {Email} that task '{Title}' was assigned to them.", user!.Email, notification.Title);
    }
}