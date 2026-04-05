namespace TaskFlow.Domain.Entities;

/// <summary>
/// Represents a workspace, which acts as a container for users and projects.
/// </summary>
public class Workspace : IEntity
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public DateTime CreatedOn { get; private set; }
    public ICollection<User> Members { get; private set; } = new List<User>();
    public ICollection<Project> Projects { get; private set; } = new List<Project>();
    private Workspace() { }

    /// <summary>
    /// Creates a new <see cref="Workspace"/> instance with the specified name.
    /// </summary>
    /// <param name="name">The name of the workspace. Cannot be null, empty, or whitespace.</param>
    /// <returns>A newly created <see cref="Workspace"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the name is null, empty, or whitespace.</exception>
    public static Workspace Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Workspace
        {
            Name = name.Trim(),
            Slug = GenerateSlug(name),
            CreatedOn = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Generates a URL-friendly slug based on the provided workspace name.
    /// </summary>
    /// <param name="name">The name to convert into a slug.</param>
    /// <returns>A lowercase, hyphen-separated string suitable for URLs.</returns>
    private static string GenerateSlug(string name) 
    {
        var baseSlug = name.Trim().ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("_", "-");

        // Take first 8 characters of a new Guid — short but unique enough
        var uniqueSuffix = Guid.NewGuid().ToString("N")[..8];

        return $"{baseSlug}-{uniqueSuffix}";
    }
}