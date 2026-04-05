using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaskFlow.Tests.Infrastructure;

namespace TaskFlow.Tests;

public class TaskTests : IClassFixture<TaskFlowWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TaskTests(TaskFlowWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<(int ProjectId, string Token)> SetupProjectAsync(
        string email, string workspace)
    {
        var token = await TestHelpers.RegisterAndLoginAsync(
            _client, email: email, workspace: workspace);
        _client.SetBearerToken(token);

        var response = await _client.PostAsJsonAsync(
            "/api/v1/projects",
            new { name = "Test Project" });
        var project = await response.Content
            .ReadFromJsonAsync<ProjectDto>();

        return (project!.Id, token);
    }

    [Fact]
    public async Task CreateTask_WithValidData_Returns201()
    {
        var (projectId, _) = await SetupProjectAsync(
            "task-create@test.com", "Task Create Workspace");

        var response = await _client.PostAsJsonAsync(
            $"/api/v1/projects/{projectId}/tasks", new
            {
                title    = "My Task",
                priority = 1
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content
            .ReadFromJsonAsync<TaskDto>();
        body!.Title.Should().Be("My Task");
        body.Status.Should().Be("Todo");
    }

    [Fact]
    public async Task CreateTask_InNonexistentProject_Returns404()
    {
        var token = await TestHelpers.RegisterAndLoginAsync(
            _client,
            email: "task-404@test.com",
            workspace: "Task 404 Workspace");
        _client.SetBearerToken(token);

        var response = await _client.PostAsJsonAsync(
            "/api/v1/projects/99999/tasks", new
            {
                title    = "My Task",
                priority = 1
            });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTasks_ReturnsPaginatedResponse()
    {
        var (projectId, _) = await SetupProjectAsync(
            "task-paged@test.com", "Task Paged Workspace");

        // Create three tasks
        for (int i = 1; i <= 3; i++)
        {
            await _client.PostAsJsonAsync(
                $"/api/v1/projects/{projectId}/tasks",
                new { title = $"Task {i}", priority = 0 });
        }

        var response = await _client.GetAsync(
            $"/api/v1/projects/{projectId}/tasks?page=1&pageSize=2");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content
            .ReadFromJsonAsync<PagedDto<TaskDto>>();
        body!.Items.Should().HaveCount(2);
        body.TotalCount.Should().Be(3);
        body.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task CompleteTask_Returns200WithDoneStatus()
    {
        var (projectId, _) = await SetupProjectAsync(
            "task-complete@test.com", "Task Complete Workspace");

        var createResponse = await _client.PostAsJsonAsync(
            $"/api/v1/projects/{projectId}/tasks",
            new { title = "Complete Me", priority = 0 });
        var task = await createResponse.Content
            .ReadFromJsonAsync<TaskDto>();

        var response = await _client.PatchAsJsonAsync(
            $"/api/v1/projects/{projectId}/tasks/{task!.Id}",
             new {Status = 3});

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content
            .ReadFromJsonAsync<TaskDto>();
        body!.Status.Should().Be("Done");
    }

    [Fact]
    public async Task DeleteTask_AsOwner_Returns204()
    {
        var (projectId, _) = await SetupProjectAsync(
            "task-delete@test.com", "Task Delete Workspace");

        var createResponse = await _client.PostAsJsonAsync(
            $"/api/v1/projects/{projectId}/tasks",
            new { title = "Delete Me", priority = 0 });
        var task = await createResponse.Content
            .ReadFromJsonAsync<TaskDto>();

        var response = await _client.DeleteAsync(
            $"/api/v1/projects/{projectId}/tasks/{task!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTask_AsNonOwner_Returns401()
    {
        var (projectId, _) = await SetupProjectAsync(
            "task-owner@test.com", "Owner Workspace");

        var createResponse = await _client.PostAsJsonAsync(
            $"/api/v1/projects/{projectId}/tasks",
            new { title = "Protected Task", priority = 0 });
        var task = await createResponse.Content
            .ReadFromJsonAsync<TaskDto>();

        // Second user tries to delete
        var secondToken = await TestHelpers.RegisterAndLoginAsync(
            _client,
            email:     "task-intruder@test.com",
            workspace: "Intruder Workspace");
        _client.SetBearerToken(secondToken);

        var response = await _client.DeleteAsync(
            $"/api/v1/projects/{projectId}/tasks/{task!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record ProjectDto(int Id);
    private record TaskDto(int Id, string Title, string Status);
    private record PagedDto<T>(
        IEnumerable<T> Items,
        int TotalCount,
        int TotalPages);
}