namespace TaskFlow.Application.Interfaces;

public interface ICurrentUserService
{
    int UserId { get; }
    int WorkspaceId { get; }
    string Email { get; }
    bool IsAuthenticated { get; }
}