using Argo.VS.CustomersApi.Domain.CustomersAggregate;
using Microsoft.EntityFrameworkCore;

namespace Argo.VS.CustomersApi.Infrastructure.Persistence;

public class CustomerDbContext(
    DbContextOptions<CustomerDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(CustomerDbContext).Assembly);
    }
}