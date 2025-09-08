namespace Argo.VS.CustomersApi.IntegrationTests.Features.Customers;

using System.Net;
using System.Net.Http.Json;

using AwesomeAssertions;

using CustomersApi.Features.Customers.Commands.CreateCustomer;
using CustomersApi.Features.Customers.Common;

using Testing;

public class CreateCustomerEndpointTests(
    CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task CreateCustomer_WhenRequestIsValid_ShouldReturnOkWithCustomerResponse()
    {
        // Arrange
        var client = factory.CreateClient();

        var request = new CreateCustomerRequest(
            "Ada",
            "Lovelace",
            "ada@example.com"
        );

        var url = "/api/v1/customers";

        // Act
        var response = await client.PostAsJsonAsync(url, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var customer = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        customer.Should().NotBeNull();
        customer.FirstName.Should().Be(request.FirstName);
        customer.LastName.Should().Be(request.LastName);
        customer.EmailAddress.Should().Be(request.EmailAddress);
        customer.Id.Should().NotBeEmpty();
    }
}
