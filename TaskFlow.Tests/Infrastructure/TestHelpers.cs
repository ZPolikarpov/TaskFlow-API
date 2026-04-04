using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace TaskFlow.Tests.Infrastructure;

public static class TestHelpers
{
    public static async Task<string> RegisterAndLoginAsync(
        HttpClient client,
        string email       = "test@taskflow.com",
        string password    = "Password123!",
        string displayName = "Test User",
        string workspace   = "Test Workspace")
    {
        // Register
        await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            email,
            password,
            displayName,
            workspaceName = workspace
        });

        // Login and extract token
        var loginResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/login", new { email, password });

        var loginBody = await loginResponse.Content
            .ReadFromJsonAsync<LoginResponseDto>();

        return loginBody!.Token;
    }

    public static void SetBearerToken(
        this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    // Local DTO — only used in tests
    private record LoginResponseDto(string Token);
}