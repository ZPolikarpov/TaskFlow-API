namespace TaskFlow.Application.DTOs.Requests;

public record RegisterRequest(
    string Email,
    string Password,
    string DisplayName,
    string WorkspaceName);