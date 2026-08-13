using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Cursos.Tests.Fixtures;

public sealed class AuthFixture
{
    private readonly HttpClient _client;

    public AuthFixture(HttpClient client)
    {
        _client = client;
    }

    public async Task<string> RegisterAndLoginAsync(
        string email,
        string password,
        string role)
    {
        var registerResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new
            {
                email,
                password,
                role
            });

        registerResponse.EnsureSuccessStatusCode();

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                email,
                password
            });

        loginResponse.EnsureSuccessStatusCode();

        var payload = await loginResponse.Content
            .ReadFromJsonAsync<LoginResponse>();

        return payload?.AccessToken
            ?? throw new InvalidOperationException(
                "O login não retornou accessToken.");
    }

    private sealed record LoginResponse(
        string AccessToken);
}