namespace Argo.VS.CustomersApi.Features.Customers.Commands.UpdateCustomer;

using Common;

using Infrastructure.CQRS;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

public class UpdateCustomerCommandHandler(
    CustomerDbContext dbContext)
    : ICommandHandler<UpdateCustomerCommand, CustomerResponse>
{
    public async Task<CustomerResponse> Handle(UpdateCustomerCommand command, CancellationToken cancellationToken)
    {
        var customer = await dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (customer == null)
        {
            throw new NotFoundException($"Customer with Id '{command.Id}' not found.");
        }

        customer.Update(
            command.FirstName,
            command.LastName,
            command.EmailAddress);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new CustomerResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.EmailAddress);
    }
}
