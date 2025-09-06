using Argo.VS.CustomersApi;
using Argo.VS.CustomersApi.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication
    .CreateBuilder(args)
    .ConfigureApplicationBuilder();

var app = builder
    .Build()
    .ConfigureApplication();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
    await dbContext.Database.MigrateAsync();
}

await app.RunAsync();
