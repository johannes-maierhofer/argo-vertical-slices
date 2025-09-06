namespace Argo.VS.CustomersApi.Features.Customers.Queries.GetCustomerList;

using Common;

using Infrastructure.CQRS;

public record GetCustomerListQuery(
    int PageNumber = 1,
    int PageSize = 10
) : IQuery<CustomerListResponse>;
