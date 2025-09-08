namespace Argo.VS.CustomersApi.IntegrationTests.Testing;

using Fixtures;

using Infrastructure.Configuration;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

using Serilog;
using Serilog.Events;

using Xunit.Abstractions;

public class CustomWebApplicationFactory(
    DatabaseFixture database,
    ITestOutputHelper output) : WebApplicationFactory<ApiRoot>
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
            services.AddSerilog(cfg =>
            {
                // customize log levels
                cfg.MinimumLevel.Is(LogEventLevel.Debug)
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Query", LogEventLevel.Information)

                    // see https://github.com/dotnet/aspnetcore/issues/46280
                    .MinimumLevel.Override("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware", LogEventLevel.Fatal)

                    // Enrichers
                    .Enrich.FromLogContext()

                    // write to test output via Serilog.Sinks.XUnit
                    .WriteTo.TestOutput(output);
            });
        });
    }
}
