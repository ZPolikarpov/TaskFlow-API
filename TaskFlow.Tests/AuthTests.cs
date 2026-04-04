using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaskFlow.Tests.Infrastructure;

namespace TaskFlow.Tests;

public class AuthTests : IClassFixture<TaskFlowWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthTests(TaskFlowWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_Returns201()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/v1/auth/register", new
            {
                email         = "alice@test.com",
                password      = "Password123!",
                displayName   = "Alice",
                workspaceName = "Alice Workspace"
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content
            .ReadFromJsonAsync<AuthResponseDto>();
        body!.Token.Should().NotBeNullOrEmpty();
        body.UserId.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns409()
    {
        var payload = new
        {
            email         = "duplicate@test.com",
            password      = "Password123!",
            displayName   = "User",
            workspaceName = "Workspace"
        };

        await _client.PostAsJsonAsync("/api/v1/auth/register", payload);
        var response = await _client.PostAsJsonAsync(
            "/api/v1/auth/register", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Register_WithInvalidEmail_Returns422()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/v1/auth/register", new
            {
                email         = "not-an-email",
                password      = "Password123!",
                displayName   = "User",
                workspaceName = "Workspace"
            });

        response.StatusCode.Should().Be(
            HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Register_WithShortPassword_Returns422()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/v1/auth/register", new
            {
                email         = "user@test.com",
                password      = "short",
                displayName   = "User",
                workspaceName = "Workspace"
            });

        response.StatusCode.Should().Be(
            HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Login_WithValidCredentials_Returns200WithToken()
    {
        await _client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            email         = "login@test.com",
            password      = "Password123!",
            displayName   = "Login User",
            workspaceName = "Login Workspace"
        });

        var response = await _client.PostAsJsonAsync(
            "/api/v1/auth/login", new
            {
                email    = "login@test.com",
                password = "Password123!"
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content
            .ReadFromJsonAsync<AuthResponseDto>();
        body!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        await _client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            email         = "wrongpass@test.com",
            password      = "Password123!",
            displayName   = "User",
            workspaceName = "Workspace"
        });

        var response = await _client.PostAsJsonAsync(
            "/api/v1/auth/login", new
            {
                email    = "wrongpass@test.com",
                password = "WrongPassword!"
            });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record AuthResponseDto(
        string Token,
        int UserId,
        string DisplayName,
        string Email);
}