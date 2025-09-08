namespace Argo.VS.CustomersApi.IntegrationTests.Testing;

using ApiClients;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

using Serilog;
using Serilog.Events;

using Xunit.Abstractions;

public static class WebApplicationFactoryExtensions
{
    public static WebApplicationFactory<ApiRoot> WithTestLogging(
        this CustomWebApplicationFactory factory,
        ITestOutputHelper output)
    {
        return factory.WithWebHostBuilder(builder =>
        {
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
        });
    }

    public static CustomersApiClient CreateApiClient(this WebApplicationFactory<ApiRoot> factory)
    {
        var httpClient = factory
            .CreateClient();

        return new CustomersApiClient(httpClient);
    }
}
