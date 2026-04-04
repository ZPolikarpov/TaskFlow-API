using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Common;

namespace TaskFlow.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(
        this Result<T> result, ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.Ok(result.Value);

        return result.ErrorType switch
        {
            ResultErrorType.NotFound     => controller.NotFound(
                ProblemFor(result.Error!, 404, "Not Found")),
            ResultErrorType.Conflict     => controller.Conflict(
                ProblemFor(result.Error!, 409, "Conflict")),
            ResultErrorType.Forbidden    => controller.StatusCode(403,
                ProblemFor(result.Error!, 403, "Forbidden")),
            ResultErrorType.Unauthorized => controller.Unauthorized(
                ProblemFor(result.Error!, 401, "Unauthorised")),
            _                            => controller.BadRequest(
                ProblemFor(result.Error!, 400, "Bad Request"))
        };
    }

    public static IActionResult ToCreatedResult<T>(
        this Result<T> result, ControllerBase controller,
        string actionName, object? routeValues)
    {
        if (!result.IsSuccess)
            return result.ToActionResult(controller);

        return controller.CreatedAtAction(actionName, routeValues, result.Value);
    }

    public static IActionResult ToNoContentResult<T>(
        this Result<T> result, ControllerBase controller)
    {
        if (!result.IsSuccess)
            return result.ToActionResult(controller);

        return controller.NoContent();
    }

    private static ProblemDetails ProblemFor(
        string detail, int status, string title) => new()
    {
        Detail = detail,
        Status = status,
        Title  = title
    };
}