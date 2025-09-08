namespace Argo.VS.CustomersApi.IntegrationTests.Testing.Fixtures;

using System.Threading.Tasks;

using Infrastructure.Persistence;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using Respawn;

using Testcontainers.MsSql;

public class DatabaseFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _mssqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithReuse(true)
        .WithLabel("reuse-id", "VsCustomerTestDb")
        .Build();

    private Respawner _respawner = null!;

    public string ConnectionString { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        await _mssqlContainer.StartAsync();

        var builder = new SqlConnectionStringBuilder(_mssqlContainer.GetConnectionString())
        {
            InitialCatalog = "VsCustomerTestDb"
        };

        this.ConnectionString = builder.ConnectionString;

        await using var dbContext = this.CreateDbContext();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();

        _respawner = await Respawner.CreateAsync(this.ConnectionString, new RespawnerOptions
        {
            TablesToIgnore = [
                "__EFMigrationsHistory"
            ],
        });
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task ResetAsync()
    {
        await _respawner.ResetAsync(this.ConnectionString);
    }

    public CustomerDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CustomerDbContext>()
            .UseSqlServer(this.ConnectionString)
            .Options;

        return new CustomerDbContext(options);
    }
}
