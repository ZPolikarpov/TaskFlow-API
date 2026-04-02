namespace TaskFlow.Domain.Exceptions;

public class NotFoundException : DomainException
{
    public string EntityType { get; }
    public object EntityId { get; }
    public NotFoundException(string entityType, object id)
        : base($"{entityType} witd id '{id}' was not found")
    {
        EntityType = entityType;
        EntityId = id;
    }
}