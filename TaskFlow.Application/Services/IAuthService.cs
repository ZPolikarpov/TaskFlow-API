using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Requests;

namespace TaskFlow.Application.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest req, CancellationToken ct = default);
    Task<Result<AuthResponse>> LoginAsync(RegisterRequest req, CancellationToken ct = default);
}