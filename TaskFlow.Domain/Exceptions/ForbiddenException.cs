namespace TaskFlow.Domain.Exceptions;

public class ForbiddenException : DomainException
{
    public ConflictException(string message) : base(message) { }
}