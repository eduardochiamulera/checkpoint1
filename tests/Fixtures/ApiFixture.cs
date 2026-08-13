using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace Cursos.Tests.Fixtures;

public sealed class ApiFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MySqlDatabaseFixture _database = new();

    public HttpClient Client => CreateClient();

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration(
            (_, configuration) =>
            {
                configuration.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:appConnection"] =
                            _database.ConnectionString,
                        ["Jwt:Key"] =
                            "test-key-minimum-32-characters-long",
                        ["Jwt:Issuer"] = "Cursos.Tests",
                        ["Jwt:Audience"] = "Cursos.Tests",
                        ["AdminUser:Password"] =
                            "Admin-Test-123!"
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
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        return request;
    }
}