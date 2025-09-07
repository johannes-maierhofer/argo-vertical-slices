namespace Argo.VS.CustomersApi.IntegrationTests.Features.Customers;

using System.Net;

using Testing.Fixtures;

using AwesomeAssertions;

using Microsoft.EntityFrameworkCore;

using Testing;
using Testing.Builders;

using Xunit.Abstractions;

using ApiClient = Testing.ApiClients;

public class CreateCustomerEndpointTests(
    DatabaseFixture database,
    ITestOutputHelper output) : IntegrationTestBase(database, output)
{
    [Fact]
    public async Task CreateCustomers_WhenRequestIsValid_ShouldReturnOkWithCustomerResponse()
    {
        // Arrange
        await using var factory = this.CreateWebAppFactory();
        var client = factory.CreateApiClient();

        // Act
        var request = new ApiClient.CreateCustomerRequest
        {
            FirstName = "Ada", LastName = "Lovelace", EmailAddress = "ada@example.com"
        };

        var response = await client.CreateCustomerAsync(request);

        // Assert: Response
        response.Should().NotBeNull();
        response.FirstName.Should().Be(request.FirstName);
        response.LastName.Should().Be(request.LastName);
        response.EmailAddress.Should().Be(request.EmailAddress);
        response.Id.Should().NotBeEmpty();

        // Assert: Db
        await using var dbContext = this.CreateDbContext();

        var customerFromDb = await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == response.Id);
        customerFromDb.Should().NotBeNull();
        customerFromDb.FirstName.Should().Be(request.FirstName);
        customerFromDb.LastName.Should().Be(request.LastName);
        customerFromDb.EmailAddress.Should().Be(request.EmailAddress);
    }

    [Fact]
    public async Task CreateCustomers_WhenEmailAlreadyExists_ShouldReturnBadRequest()
    {
        // Arrange
        const string existingEmail = "existing-email@test.com";

        var existingCustomer = new CustomerBuilder()
            .WithEmailAddress(existingEmail)
            .Build();

        await this.AddEntityToDb(existingCustomer);

        await using var factory = this.CreateWebAppFactory();
        var client = factory.CreateApiClient();

        var request = new ApiClient.CreateCustomerRequest
        {
            FirstName = "Another",
            LastName = "User",
            EmailAddress = existingEmail
        };

        // Act
        var action = () => client.CreateCustomerAsync(request);

        // Assert
        var errorResult = await action.Should()
            .ThrowAsync<ApiClient.ApiException<ApiClient.ProblemDetails>>();

        errorResult.And.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }
}
