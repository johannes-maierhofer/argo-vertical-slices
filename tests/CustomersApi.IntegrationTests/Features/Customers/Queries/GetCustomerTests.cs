namespace Argo.VS.CustomersApi.IntegrationTests.Features.Customers.Queries;

using System.Net;

using Testing;
using Testing.Builders;
using Testing.Fixtures;

using AwesomeAssertions;

using Xunit.Abstractions;

using ApiClient = Testing.ApiClients;

public class GetCustomerEndpointTests(
    DatabaseFixture database,
    ITestOutputHelper output) : IntegrationTestBase(database, output)
{
    [Fact]
    public async Task GetCustomer_WhenCustomerExists_ShouldReturnOkWithCustomerResponse()
    {
        // Arrange
        var existing = new CustomerBuilder()
            .WithFirstName("Ada")
            .WithLastName("Lovelace")
            .WithEmailAddress("ada@example.com")
            .Build();

        await this.AddEntityToDb(existing);

        await using var factory = this.CreateWebAppFactory();
        var client = factory.CreateApiClient();

        // Act
        var response = await client.GetCustomerAsync(existing.Id);

        // Assert: Response
        response.Should().NotBeNull();
        response.Id.Should().Be(existing.Id);
        response.FirstName.Should().Be(existing.FirstName);
        response.LastName.Should().Be(existing.LastName);
        response.EmailAddress.Should().Be(existing.EmailAddress);
    }

    [Fact]
    public async Task GetCustomer_WhenCustomerDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        await using var factory = this.CreateWebAppFactory();
        var client = factory.CreateApiClient();
        var missingId = Guid.NewGuid();

        // Act
        var act = () => client.GetCustomerAsync(missingId);

        // Assert
        var error = await act.Should()
            .ThrowAsync<ApiClient.ApiException<ApiClient.ProblemDetails>>();

        error.And.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }
}
