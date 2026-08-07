using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Testcontainers.MySql;

namespace Cursos.Tests.Fixtures;

public sealed class MySqlDatabaseFixture : IAsyncLifetime
{
    private readonly MySqlContainer _container =
        new MySqlBuilder()
            .WithDatabase("cursos_tests")
            .WithUsername("root")
            .WithPassword("test_password")
            .WithImage("mysql:8.0")
            .Build();

    public string ConnectionString =>
        _container.GetConnectionString();

    public Task InitializeAsync() =>
        _container.StartAsync();

    public Task DisposeAsync() =>
        _container.DisposeAsync().AsTask();
}