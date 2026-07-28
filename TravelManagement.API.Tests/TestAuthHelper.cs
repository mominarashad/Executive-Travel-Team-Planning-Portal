using System.Net.Http.Json;

namespace TravelManagement.API.Tests;

public static class TestAuthHelper
{
    public static async Task<string> GetTokenAsync(HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
        
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<LoginResult>();
        return body!.token;
    }

    public static async Task<HttpClient> GetAuthenticatedClientAsync(
        CustomWebApplicationFactory factory, string email, string password)
    {
        var client = factory.CreateClient();
        var token = await GetTokenAsync(client, email, password);
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private record LoginResult(string token);
}