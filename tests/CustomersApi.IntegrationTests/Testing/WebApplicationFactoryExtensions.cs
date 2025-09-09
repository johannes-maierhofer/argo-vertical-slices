namespace Argo.VS.CustomersApi.IntegrationTests.Testing;

using ApiClients;

using Microsoft.AspNetCore.Mvc.Testing;

public static class WebApplicationFactoryExtensions
{
    public static CustomersApiClient CreateApiClient(this WebApplicationFactory<ApiRoot> factory)
    {
        var httpClient = factory
            .CreateClient();

        return new CustomersApiClient(httpClient);
    }
}
