namespace TaskFlow.Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public ResultErrorType ErrorType { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    private Result(string error, ResultErrorType type)
    {
        IsSuccess = false;
        Error = error;
        ErrorType = type;
    }

    public static Result<T> Success(T value) => new(value);

    public static Result<T> NotFound(string message) =>
        new(message, ResultErrorType.NotFound);

    public static Result<T> Conflict(string message) =>
        new(message, ResultErrorType.Conflict);

    public static Result<T> Forbidden(string message) =>
        new(message, ResultErrorType.Forbidden);

    public static Result<T> Unauthorized(string message) =>
        new(message, ResultErrorType.Unauthorized);

    public static Result<T> Failure(string message) =>
        new(message, ResultErrorType.Failure);
}

public enum ResultErrorType
{
    NotFound,
    Conflict,
    Forbidden,
    Unauthorized,
    Failure
}