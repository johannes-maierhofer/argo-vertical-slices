namespace Argo.VS.CustomersApi.IntegrationTests.Features.Customers.Commands;

using System.Net;

using Testing.Fixtures;

using AwesomeAssertions;

using Microsoft.EntityFrameworkCore;

using Testing;
using Testing.Builders;

using Xunit.Abstractions;

using ApiClient = Testing.ApiClients;

public class DeleteCustomerEndpointTests(
    DatabaseFixture database,
    ITestOutputHelper output) : IntegrationTestBase(database, output)
{
    [Fact]
    public async Task DeleteCustomer_WhenCustomerExists_ShouldReturnNoContentAndRemoveFromDb()
    {
        // Arrange
        var existing = new CustomerBuilder()
            .WithFirstName("To")
            .WithLastName("Delete")
            .WithEmailAddress("delete-me@example.com")
            .Build();

        await this.AddEntityToDb(existing);

        await using var factory = this.CreateWebAppFactory();
        var client = factory.CreateApiClient();

        // Act
        var act = () => client.DeleteCustomerAsync(existing.Id);

        // Assert: HTTP
        await act.Should().NotThrowAsync(); // 204 No Content yields no exception in typical generated clients

        // Assert: Db
        await using var dbContext = this.CreateDbContext();
        var fromDb = await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == existing.Id);
        fromDb.Should().BeNull();
    }

    [Fact]
    public async Task DeleteCustomer_WhenCustomerDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        await using var factory = this.CreateWebAppFactory();
        var client = factory.CreateApiClient();

        var missingId = Guid.NewGuid();

        // Act
        var act = () => client.DeleteCustomerAsync(missingId);

        // Assert
        var error = await act.Should()
            .ThrowAsync<ApiClient.ApiException<ApiClient.ProblemDetails>>();

        error.And.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCustomer_WhenCalledTwice_ShouldReturnNotFoundOnSecondCall()
    {
        // Arrange
        var existing = new CustomerBuilder()
            .WithFirstName("Twice")
            .WithLastName("Deleted")
            .WithEmailAddress("twice@example.com")
            .Build();

        await this.AddEntityToDb(existing);

        await using var factory = this.CreateWebAppFactory();
        var client = factory.CreateApiClient();

        // Act 1: first delete should succeed
        await client.DeleteCustomerAsync(existing.Id);

        // Assert DB after first delete
        await using (var verify = this.CreateDbContext())
        {
            var gone = await verify.Customers.FirstOrDefaultAsync(c => c.Id == existing.Id);
            gone.Should().BeNull();
        }

        // Act 2: second delete should 404
        var second = () => client.DeleteCustomerAsync(existing.Id);

        // Assert
        var error = await second.Should()
            .ThrowAsync<ApiClient.ApiException<ApiClient.ProblemDetails>>();

        error.And.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }
}
