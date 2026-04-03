using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Interfaces;

public interface ITokenService
{
    public string GetToken(User user);
}