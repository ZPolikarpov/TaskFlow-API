using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Extensions;
using TaskFlow.Application.DTOs.Requests;
using TaskFlow.Application.Services;

namespace TaskFlow.Api.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/projects/{projectId}/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _tasks;

    public TasksController(ITaskService tasks)
    {
        _tasks = tasks;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(
        int projectId,
        [FromQuery] int page     = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct     = default)
    {
        var result = await _tasks.GetPagedAsync(projectId, page, pageSize, ct);
        return result.ToActionResult(this);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        int projectId,
        int id,
        CancellationToken ct     = default)
    {
        var result = await _tasks.GetByIdAsync(id, ct);
        return result.ToActionResult(this);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        int projectId,
        CreateTaskRequest request,
        CancellationToken ct)
    {
        var command = new CreateTaskCommand(
            request.Title,
            request.Priority,
            projectId,
            request.Description,
            request.AssigneeId,
            request.DueDate,
            request.Status
        );
        var result = await _tasks.CreateAsync(command, ct);
        return result.ToCreatedResult(this, nameof(GetById),
            new { projectId, id = result.Value?.Id });
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        int projectId, int id, 
        UpdateTaskRequest req, 
        CancellationToken ct)
    {
        var result = await _tasks.UpdateAsync(id, req, ct);
        return result.ToActionResult(this);
    }

    [HttpPatch("{id:int}/assign")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Assign(
        int projectId, int id,
        AssignTaskRequest request,
        CancellationToken ct)
    {
        var result = await _tasks.AssignAsync(id, request, ct);
        return result.ToActionResult(this);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        int projectId, int id, CancellationToken ct)
    {
        var result = await _tasks.DeleteAsync(id, ct);
        return result.ToNoContentResult(this);
    }
}