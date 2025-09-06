namespace Argo.VS.CustomersApi.Features.Customers.Queries.GetCustomerList;

using Common;

using Infrastructure.CQRS;
using Infrastructure.Extensions;
using Infrastructure.Persistence;

public class GetCustomerListQueryHandler(
    CustomerDbContext dbContext)
    : IQueryHandler<GetCustomerListQuery, CustomerListResponse>
{
    public async Task<CustomerListResponse> Handle(GetCustomerListQuery query, CancellationToken cancellationToken)
    {
        var result = await dbContext
            .Customers
            .Select(c => new CustomerListItem(
                c.Id,
                c.FirstName,
                c.LastName,
                c.EmailAddress))
            .ToPaginatedListAsync(
                query.PageNumber,
                query.PageSize,
                cancellationToken);

        return new CustomerListResponse(
            result.Items.ToList(),
            result.PageNumber,
            result.TotalPages,
            result.TotalCount);
    }
}
