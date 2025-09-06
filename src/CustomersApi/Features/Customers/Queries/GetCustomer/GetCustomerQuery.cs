namespace Argo.VS.CustomersApi.Features.Customers.Queries.GetCustomer;

using Common;

using Infrastructure.CQRS;

public record GetCustomerQuery(
    Guid CustomerId) : IQuery<CustomerResponse>;
