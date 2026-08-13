using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Cursos.Tests.Fixtures;

/// <summary>
/// Boots the real Cursos.API (Program) in-memory for integration tests,
/// backed by a disposable MySQL container (see <see cref="MySqlDatabaseFixture"/>).
/// Configuration keys below MUST match what src/Cursos.API/Program.cs actually reads:
///   - ConnectionStrings:DefaultConnection
///   - JwtSettings:SecretKey / JwtSettings:Issuer / JwtSettings:Audience / JwtSettings:ExpirationMinutes
///   - PaymentGateway:Type
/// </summary>
public sealed class ApiFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MySqlDatabaseFixture _database = new();

    public HttpClient Client => CreateClient();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = _database.ConnectionString,
                    ["JwtSettings:SecretKey"] = "test-key-minimum-32-characters-long!!",
                    ["JwtSettings:Issuer"] = "Cursos.Tests",
                    ["JwtSettings:Audience"] = "Cursos.Tests",
                    ["JwtSettings:ExpirationMinutes"] = "60",
                    ["PaymentGateway:Type"] = "Simulated"
                });
        });
    }

    public async Task InitializeAsync()
    {
        await _database.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        await _database.DisposeAsync();
        Dispose();
    }

    public HttpRequestMessage AuthenticatedRequest(
        HttpMethod method,
        string path,
        string token)
    {
        var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }
}
