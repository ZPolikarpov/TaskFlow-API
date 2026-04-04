using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaskFlow.Tests.Infrastructure;

namespace TaskFlow.Tests;

public class ProjectTests : IClassFixture<TaskFlowWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProjectTests(TaskFlowWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProjects_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/api/v1/projects");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateProject_WithValidData_Returns201()
    {
        var token = await TestHelpers.RegisterAndLoginAsync(
            _client,
            email: "project-create@test.com",
            workspace: "Project Create Workspace");
        _client.SetBearerToken(token);

        var response = await _client.PostAsJsonAsync(
            "/api/v1/projects", new
            {
                name        = "My Project",
                description = "A test project"
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content
            .ReadFromJsonAsync<ProjectResponseDto>();
        body!.Name.Should().Be("My Project");
        body.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetProjectById_WhenExists_Returns200()
    {
        var token = await TestHelpers.RegisterAndLoginAsync(
            _client,
            email: "project-get@test.com",
            workspace: "Project Get Workspace");
        _client.SetBearerToken(token);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/v1/projects", new { name = "Fetch Me" });
        var created = await createResponse.Content
            .ReadFromJsonAsync<ProjectResponseDto>();

        var response = await _client
            .GetAsync($"/api/v1/projects/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content
            .ReadFromJsonAsync<ProjectResponseDto>();
        body!.Name.Should().Be("Fetch Me");
    }

    [Fact]
    public async Task GetProjectById_WhenNotExists_Returns404()
    {
        var token = await TestHelpers.RegisterAndLoginAsync(
            _client,
            email: "project-404@test.com",
            workspace: "Project 404 Workspace");
        _client.SetBearerToken(token);

        var response = await _client.GetAsync("/api/v1/projects/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ArchiveProject_AsOwner_Returns200()
    {
        var token = await TestHelpers.RegisterAndLoginAsync(
            _client,
            email: "archive@test.com",
            workspace: "Archive Workspace");
        _client.SetBearerToken(token);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/v1/projects", new { name = "Archive Me" });
        var created = await createResponse.Content
            .ReadFromJsonAsync<ProjectResponseDto>();

        var response = await _client.PatchAsync(
            $"/api/v1/projects/{created!.Id}/archive", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content
            .ReadFromJsonAsync<ProjectResponseDto>();
        body!.IsArchived.Should().BeTrue();
    }

    [Fact]
    public async Task CreateProject_WithEmptyName_Returns422()
    {
        var token = await TestHelpers.RegisterAndLoginAsync(
            _client,
            email: "project-validation@test.com",
            workspace: "Validation Workspace");
        _client.SetBearerToken(token);

        var response = await _client.PostAsJsonAsync(
            "/api/v1/projects", new { name = "" });

        response.StatusCode.Should().Be(
            HttpStatusCode.UnprocessableEntity);
    }

    private record ProjectResponseDto(
        int Id,
        string Name,
        bool IsArchived);
}