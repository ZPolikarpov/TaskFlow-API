namespace TaskFlow.Domain.Exceptions;

public abstract class DomainException : Exception
{
    public static DomainException(string message) : base(message) {}
    public static DomainException(string message, Exception innerException) : base(message: message, innerException: innerException) {}
}