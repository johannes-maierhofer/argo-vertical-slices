namespace Argo.VS.CustomersApi.Infrastructure.Configuration;

using System;

public static class CustomEnvironmentExtensions
{
    public static bool IsIntegrationTests(this IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(environment);

        return environment.IsEnvironment(CustomEnvironments.IntegrationTests);
    }
}
