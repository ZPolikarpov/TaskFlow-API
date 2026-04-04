namespace TaskFlow.Application.DTOs.Responses;

public record AuthResponse(
    string Token,
    DateTime ExpiresAt,
    int UserId,
    string DisplayName,
    string Email);