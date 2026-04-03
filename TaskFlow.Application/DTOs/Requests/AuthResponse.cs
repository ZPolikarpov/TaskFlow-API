namespace TaskFlow.Application.DTOs.Requests;

public record AuthResponse(
    string Token,
    DateTime ExpiresAt,
    int UserId,
    string DisplayName,
    string Email);