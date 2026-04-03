using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Requests;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Services;

public class AuthService
{
    private readonly IUserRepository _users;
    private readonly IRepository<Workspace> _workspaces;
    private readonly ITokenService _tokenService;
    public AuthService(
        IUserRepository users, 
        IRepository<Workspace> workspaces, 
        ITokenService tokenService
    )
    {
      _users = users;
      _workspaces = workspaces;
      _tokenService = tokenService;  
    }
    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest req, CancellationToken ct = default)
    {
        bool exists = await _users.ExistsByEmailAsync(req.Email, ct);
        if (exists)
            return Result<AuthResponse>.Conflict($"User with email {req.Email} already exists!");

        Workspace workspace = Workspace.Create(req.WorkspaceName);
        workspace = await _workspaces.AddAsync(workspace);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);

        User user = User.Create(
            req.DisplayName,
            req.Email,
            passwordHash,
            workspace.Id
        );
        await _users.AddAsync(user);

        var token = _tokenService.Issue(user);
        var expiry = _tokenService.GetExpiry();      

        return Result<AuthResponse>.Success(
            new AuthResponse(token, expiry, user.Id, user.DisplayName, user.Email)
        );
    }

    public async Task<Result<AuthResponse>> LoginAsync(RegisterRequest req, CancellationToken ct = default)
    {
        var user = await _users.FindByEmailAsync(req.Email);
        if (user is null)
        {
            // Hash a dummy password to mask response times
           BCrypt.Net.BCrypt.HashPassword("DummyPassword");
           return Result<AuthResponse>.NotFound($"Could not find user with given credentials.");
        }

        if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Result<AuthResponse>.NotFound($"Could not find user with given credentials.");

        var token = _tokenService.Issue(user);
        var expiry = _tokenService.GetExpiry();   

        return Result<AuthResponse>.Success(
            new AuthResponse(token, expiry, user.Id, user.DisplayName, user.Email)
        );
    }
}