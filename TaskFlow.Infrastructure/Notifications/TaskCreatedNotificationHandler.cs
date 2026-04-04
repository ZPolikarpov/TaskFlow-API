using Microsoft.Extensions.Logging;
using TaskFlow.Application.BackgroundJobs;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Infrastructure.Notifications;

public class TaskCreatedNotificationHandler : INotificationHandler<TaskCreatedNotification>
{
    private readonly IUserRepository _users;
    private readonly ILogger<TaskCreatedNotificationHandler> _logger;

    public TaskCreatedNotificationHandler(
        IUserRepository users,
        ILogger<TaskCreatedNotificationHandler> logger
    )
    {
        _users = users;
        _logger = logger;
    }
    public async Task HandleAsync(TaskCreatedNotification notification, CancellationToken ct)
    {
        if (notification.AssigneeId is null)
            return;
        
        int userId = (int)notification.AssigneeId;
        var user = await _users.GetByIdAsync(userId, ct);

        if (user is null)
        {
            _logger.LogWarning("Couldn't find a user with the id {userId} to send notification mail.", userId);
            return;
        }

        _logger.LogInformation("Notifying user {Email} that task '{Title}' was assigned to them.", user!.Email, notification.Title);
    }
}