namespace Argo.VS.CustomersApi.IntegrationTests.Testing;

using Fixtures;

using Infrastructure.Persistence;

using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;

[Collection("IntegrationTests")]
public abstract class IntegrationTestBase(
    DatabaseFixture database,
    ITestOutputHelper output)
    : IAsyncLifetime
{
    public ITestOutputHelper Output { get; } = output;

    public async Task InitializeAsync()
    {
        // reset the Db before every single test
        await database.ResetAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected WebApplicationFactory<Program> CreateWebAppFactory()
    {
        return new CustomWebApplicationFactory(database)
            .WithTestLogging(output);
    }

    protected async Task AddEntityToDb<T>(T entity)
        where T : class
    {
        await using var dbContext = database.CreateDbContext();
        await dbContext.Set<T>().AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }

    protected async Task AddEntityRangeToDb<T>(IEnumerable<T> entities)
        where T : class
    {
        await using var dbContext = database.CreateDbContext();
        await dbContext.Set<T>().AddRangeAsync(entities);
        await dbContext.SaveChangesAsync();
    }

    protected CustomerDbContext CreateDbContext() => database.CreateDbContext();
}
