namespace Argo.VS.CustomersApi.Features.Customers.Commands.DeleteCustomer;

using Infrastructure.CQRS;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

public class DeleteCustomerCommandHandler(
    CustomerDbContext dbContext)
    : ICommandHandler<DeleteCustomerCommand>
{
    public async Task Handle(DeleteCustomerCommand command, CancellationToken cancellationToken)
    {
        var customer = await dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (customer == null)
        {
            throw new NotFoundException($"Customer with Id '{command.Id}' not found.");
        }

        dbContext.Remove(customer);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
