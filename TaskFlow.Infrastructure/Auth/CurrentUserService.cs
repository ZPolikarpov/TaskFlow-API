using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TaskFlow.Application.Interfaces;

namespace TaskFlow.Infrastructure.Auth;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    private ClaimsPrincipal? User => _accessor.HttpContext?.User;

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;

    public int UserId =>
        int.Parse(User?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException(
                "UserId claim not present — is the endpoint protected?"));

    public int WorkspaceId =>
        int.Parse(User?.FindFirstValue("workspace_id")
            ?? throw new InvalidOperationException(
                "workspace_id claim not present — is the endpoint protected?"));

    public string Email =>
        User?.FindFirstValue(ClaimTypes.Email)
            ?? throw new InvalidOperationException(
                "Email claim not present — is the endpoint protected?");
}