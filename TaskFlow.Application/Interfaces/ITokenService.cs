using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Interfaces;

public interface ITokenService
{
    string Issue(User user);
    DateTime GetExpiry();
}