namespace Argo.VS.CustomersApi.IntegrationTests.Testing;

using Fixtures;

using Infrastructure.Configuration;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

public class CustomWebApplicationFactory(DatabaseFixture database) : WebApplicationFactory<ApiRoot>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(CustomEnvironments.IntegrationTests);

        builder.ConfigureAppConfiguration((_, config) =>
        {
            var testConfig = new Dictionary<string, string?>
            {
                { "ConnectionStrings:CustomerDb", database.ConnectionString },
            };

            config.AddInMemoryCollection(testConfig);
        });

        builder.ConfigureTestServices(services =>
        {
            // remove Background services
            services.RemoveAll(typeof(IHostedService));
        });
    }
}
