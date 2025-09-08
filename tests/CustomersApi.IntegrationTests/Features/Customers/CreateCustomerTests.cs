namespace Argo.VS.CustomersApi.IntegrationTests.Features.Customers;

using System.Net;
using System.Net.Http.Json;

using Testing.Fixtures;

using AwesomeAssertions;

using CustomersApi.Features.Customers.Commands.CreateCustomer;
using CustomersApi.Features.Customers.Common;

using Microsoft.EntityFrameworkCore;

using Testing;
using Testing.Builders;

using Xunit.Abstractions;

public class CreateCustomerEndpointTests(
    DatabaseFixture database,
    ITestOutputHelper output) : IntegrationTestBase(database, output)
{
    [Fact]
    public async Task CreateCustomer_WhenRequestIsValid_ShouldReturnOkWithCustomerResponse()
    {
        // Arrange
        await using var factory = this.CreateWebAppFactory();
        var client = factory.CreateClient();

        var request = new CreateCustomerRequest(
            "Ada",
            "Lovelace",
            "ada@example.com"
        );

        var url = "/api/v1/customers";

        // Act
        var response = await client.PostAsJsonAsync(url, request);

        // Assert: Response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var customer = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        customer.Should().NotBeNull();
        customer.FirstName.Should().Be(request.FirstName);
        customer.LastName.Should().Be(request.LastName);
        customer.EmailAddress.Should().Be(request.EmailAddress);
        customer.Id.Should().NotBeEmpty();

        // Assert: Db
        await using var dbContext = this.CreateDbContext();

        var customerFromDb = await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == customer.Id);
        customerFromDb.Should().NotBeNull();
        customerFromDb.FirstName.Should().Be(request.FirstName);
        customerFromDb.LastName.Should().Be(request.LastName);
        customerFromDb.EmailAddress.Should().Be(request.EmailAddress);
    }

    [Fact]
    public async Task CreateCustomer_WhenEmailAlreadyExists_ShouldReturnBadRequest()
    {
        // Arrange
        const string existingEmail = "existing-email@test.com";

        var existingCustomer = new CustomerBuilder()
            .WithEmailAddress(existingEmail)
            .Build();

        await this.AddEntityToDb(existingCustomer);

        await using var factory = this.CreateWebAppFactory();
        var client = factory.CreateClient();

        var request = new CreateCustomerRequest(
            "Another",
            "User",
            existingEmail
        );

        var url = "/api/v1/customers";

        // Act
        var response = await client.PostAsJsonAsync(url, request);

        // Assert: Response
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
