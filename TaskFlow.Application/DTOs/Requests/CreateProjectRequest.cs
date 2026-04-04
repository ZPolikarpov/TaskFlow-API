namespace TaskFlow.Application.DTOs.Requests;

public record CreateProjectRequest(
    string Name,
    string? Description);