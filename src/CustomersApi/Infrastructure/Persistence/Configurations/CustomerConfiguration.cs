namespace Argo.VS.CustomersApi.Infrastructure.Persistence.Configurations;

using Domain.CustomersAggregate;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.FirstName).HasMaxLength(200);
        builder.Property(c => c.LastName).HasMaxLength(200);
        builder.Property(c => c.EmailAddress).HasMaxLength(320);
    }
}