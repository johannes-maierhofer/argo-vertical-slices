namespace Argo.VS.CustomersApi.Features.Customers.Queries.GetCustomer;

using Common;

using Infrastructure.CQRS;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

public class GetCustomerQueryHandler(
    CustomerDbContext dbContext) : IQueryHandler<GetCustomerQuery, CustomerResponse>
{
    public async Task<CustomerResponse> Handle(GetCustomerQuery query, CancellationToken cancellationToken)
    {
        var result = await dbContext
            .Customers
            .Where(c => c.Id == query.CustomerId)
            .Select(c => new CustomerResponse(
                c.Id,
                c.FirstName,
                c.LastName,
                c.EmailAddress))
            .FirstOrDefaultAsync(cancellationToken);

        return result ?? throw new NotFoundException($"Customer with id '{query.CustomerId}' not found.");
    }
}
