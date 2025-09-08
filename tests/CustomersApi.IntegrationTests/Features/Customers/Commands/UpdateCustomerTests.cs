namespace Argo.VS.CustomersApi.IntegrationTests.Features.Customers.Commands;

using System.Net;

using Testing.Fixtures;

using AwesomeAssertions;

using Microsoft.EntityFrameworkCore;

using Testing;
using Testing.Builders;

using Xunit.Abstractions;

using ApiClient = Testing.ApiClients;

public class UpdateCustomerEndpointTests(
    DatabaseFixture database,
    ITestOutputHelper output) : IntegrationTestBase(database, output)
{
    [Fact]
    public async Task UpdateCustomer_WhenRequestIsValid_ShouldReturnOkWithUpdatedCustomerResponse()
    {
        // Arrange
        var existing = new CustomerBuilder()
            .WithFirstName("Grace")
            .WithLastName("Hopper")
            .WithEmailAddress("grace@example.com")
            .Build();

        await this.AddEntityToDb(existing);

        await using var factory = this.CreateWebAppFactory();
        var client = factory.CreateApiClient();

        var request = new ApiClient.UpdateCustomerRequest
        {
            FirstName = "Ada",
            LastName = "Lovelace",
            EmailAddress = "ada@example.com"
        };

        // Act
        var response = await client.UpdateCustomerAsync(existing.Id, request);

        // Assert: Response
        response.Should().NotBeNull();
        response.Id.Should().Be(existing.Id);
        response.FirstName.Should().Be(request.FirstName);
        response.LastName.Should().Be(request.LastName);
        response.EmailAddress.Should().Be(request.EmailAddress);

        // Assert: Db
        await using var dbContext = this.CreateDbContext();
        var customerFromDb = await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == existing.Id);
        customerFromDb.Should().NotBeNull();
        customerFromDb.FirstName.Should().Be(request.FirstName);
        customerFromDb.LastName.Should().Be(request.LastName);
        customerFromDb.EmailAddress.Should().Be(request.EmailAddress);
    }

    [Fact]
    public async Task UpdateCustomer_WhenCustomerDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        await using var factory = this.CreateWebAppFactory();
        var client = factory.CreateApiClient();

        var missingId = Guid.NewGuid();
        var request = new ApiClient.UpdateCustomerRequest
        {
            FirstName = "No",
            LastName = "Body",
            EmailAddress = "nobody@example.com"
        };

        // Act
        var action = () => client.UpdateCustomerAsync(missingId, request);

        // Assert
        var error = await action.Should()
            .ThrowAsync<ApiClient.ApiException<ApiClient.ProblemDetails>>();

        error.And.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateCustomer_WhenEmailAlreadyExists_ShouldReturnBadRequest()
    {
        // Arrange
        const string existingEmail = "already@taken.com";

        var customerWithEmail = new CustomerBuilder()
            .WithFirstName("Existing")
            .WithLastName("Owner")
            .WithEmailAddress(existingEmail)
            .Build();

        var toUpdate = new CustomerBuilder()
            .WithFirstName("Will")
            .WithLastName("Change")
            .WithEmailAddress("original@example.com")
            .Build();

        await this.AddEntityToDb(customerWithEmail);
        await this.AddEntityToDb(toUpdate);

        await using var factory = this.CreateWebAppFactory();
        var client = factory.CreateApiClient();

        var request = new ApiClient.UpdateCustomerRequest
        {
            FirstName = "Updated",
            LastName = "User",
            EmailAddress = existingEmail // collide with existing
        };

        // Act
        var action = () => client.UpdateCustomerAsync(toUpdate.Id, request);

        // Assert: HTTP
        var error = await action.Should()
            .ThrowAsync<ApiClient.ApiException<ApiClient.ProblemDetails>>();

        error.And.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

        // Assert: Db unchanged
        await using var dbContext = this.CreateDbContext();
        var reloaded = await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == toUpdate.Id);
        reloaded.Should().NotBeNull();
        reloaded.EmailAddress.Should().Be("original@example.com");
        reloaded.FirstName.Should().Be(toUpdate.FirstName);
        reloaded.LastName.Should().Be(toUpdate.LastName);
    }
}
