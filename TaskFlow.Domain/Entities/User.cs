namespace TaskFlow.Domain.Entities;
public class User
{
    public int Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public int WorkspaceId { get; private set; }
    public DateTimeOffset CreatedOn { get; private set; }

    public static User Create(
        string displayName, string email,
        string passwordHash, int workspaceId
    )
    {
        ArgumentException.ThrowIfNullOrEmpty(displayName);
        ArgumentException.ThrowIfNullOrEmpty(email);
        ArgumentException.ThrowIfNullOrEmpty(passwordHash);

        return new User()
        {
            DisplayName = displayName.Trim(),
            Email = email.Trim(),
            Password = passwordHash,
            WorkspaceId = workspaceId,
            CreatedOn = DateTimeOffset.UtcNow
        };
    }
}