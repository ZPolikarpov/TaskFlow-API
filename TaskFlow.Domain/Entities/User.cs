namespace TaskFlow.Domain.Entities;

/// <summary>
/// Represents a user within the system, including authentication and workspace association.
/// </summary>
public class User : IEntity
{
    public int Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public int WorkspaceId { get; private set; }
    public DateTimeOffset CreatedOn { get; private set; }

    /// <summary>
    /// Creates a new <see cref="User"/> instance with the specified credentials and profile information.
    /// </summary>
    /// <param name="displayName">The display name of the user. Cannot be null or empty.</param>
    /// <param name="email">The email address of the user. Cannot be null or empty.</param>
    /// <param name="passwordHash">The hashed password of the user. Cannot be null or empty.</param>
    /// <param name="workspaceId">The identifier of the workspace the user belongs to.</param>
    /// <returns>A newly created <see cref="User"/> instance.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="displayName"/>, <paramref name="email"/>, or <paramref name="passwordHash"/> is null or empty.
    /// </exception>
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
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            WorkspaceId = workspaceId,
            CreatedOn = DateTimeOffset.UtcNow
        };
    }
}