namespace TaskFlow.Domain.Entities;
public class Workspace
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public DateTimeOffset CreatedOn { get; private set; }
    public ICollection<User> Members { get; private set; } = new List<User>();
    public ICollection<Project> Projects { get; private set; } = new List<Project>();
    private Workspace() { }

    public static Workspace Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Workspace
        {
            Name = name.Trim(),
            Slug = GenerateSlug(name),
            CreatedOn = DateTimeOffset.UtcNow
        };
    }

    private static string GenerateSlug(string name) =>
        name.Trim().ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("_", "-");
}