using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Requests;
using TaskFlow.Application.DTOs.Responses;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projects;
    private readonly ICurrentUserService _currentUser;
    public ProjectService(
        IProjectRepository projects,
        ICurrentUserService currentUser
    )
    {
        _projects = projects;
        _currentUser = currentUser;
    }
    public async Task<Result<ProjectResponse>> CreateAsync(CreateProjectRequest req, CancellationToken ct = default)
    {
        Project project = Project.Create(
            req.Name,
            _currentUser.WorkspaceId,
            _currentUser.UserId,
            req.Description
        );
        await _projects.AddAsync(project, ct);

        return Result<ProjectResponse>.Success(ProjectResponse.From(project));
    }
    public async Task<Result<ProjectResponse>> GetByIdAsync(int projectId, CancellationToken ct = default)
    {
        var project = await _projects.GetByIdAsync(projectId, ct);

        if (project is null || project.WorkspaceId != _currentUser.WorkspaceId)
            return Result<ProjectResponse>.NotFound($"Project with Id {projectId} could not be found!");

        return Result<ProjectResponse>.Success(ProjectResponse.From(project!));
    }
    public async Task<Result<IEnumerable<ProjectResponse>>> GetAllAsync(CancellationToken ct = default)
    {
        var projects = await _projects.GetByWorkspaceAsync(_currentUser.WorkspaceId, ct);

        var responses = projects.Select(ProjectResponse.From);

        return Result<IEnumerable<ProjectResponse>>.Success(responses);
    }
    public async Task<Result<ProjectResponse>> ArchiveAsync(int projectId, CancellationToken ct = default)
    {
        var project = await _projects.GetByIdAsync(projectId, ct);
        if (project is null || project.WorkspaceId != _currentUser.WorkspaceId)
            return Result<ProjectResponse>.NotFound($"Project with Id {projectId} could not be found!");

        if (project.OwnerId != _currentUser.UserId)
            return Result<ProjectResponse>.Forbidden(
                "Only the project owner can archive a project");

        project.Archive();
        await _projects.UpdateAsync(project, ct);

        return Result<ProjectResponse>.Success(ProjectResponse.From(project));
    }
}