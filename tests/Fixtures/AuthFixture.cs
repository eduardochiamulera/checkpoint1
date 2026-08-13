using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Cursos.Tests.Fixtures;

/// <summary>
/// Registers and logs in a user through the real Cursos.API auth endpoints,
/// returning the JWT access token.
/// Routes and payloads MUST match src/Cursos.API/Controllers/AuthController.cs:
///   POST /api/auth/register  { email, password, name, phone? }
///   POST /api/auth/login     { email, password }
/// Response shape matches Cursos.Application.Auth.AuthResultDto (property: Token).
/// NOTE: the current User entity has no assignable "role" on registration -
/// every new user gets the default "User" role.
/// </summary>
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
        string name)
    {
        var registerResponse = await _client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                email,
                password,
                name
            });

        registerResponse.EnsureSuccessStatusCode();

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                email,
                password
            });

        loginResponse.EnsureSuccessStatusCode();

        var payload = await loginResponse.Content
            .ReadFromJsonAsync<AuthResultTestModel>();

        return payload?.Token
            ?? throw new InvalidOperationException("O login nao retornou token.");
    }

    private sealed record AuthResultTestModel(
        bool Success,
        string? Token,
        string? RefreshToken,
        string? ErrorMessage);
}
