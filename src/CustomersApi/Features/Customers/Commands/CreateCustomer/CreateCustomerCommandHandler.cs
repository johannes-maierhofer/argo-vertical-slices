namespace Argo.VS.CustomersApi.Features.Customers.Commands.CreateCustomer;

using Common;
using Domain.CustomersAggregate;
using Infrastructure.CQRS;
using Infrastructure.Persistence;

public class CreateCustomerCommandHandler(CustomerDbContext dbContext)
    : ICommandHandler<CreateCustomerCommand, CustomerResponse>
{
    public async Task<CustomerResponse> Handle(CreateCustomerCommand cmd, CancellationToken cancellationToken)
    {
        var customer = Customer.Create(
            cmd.FirstName,
            cmd.LastName,
            cmd.EmailAddress);

        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CustomerResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.EmailAddress);
    }
}