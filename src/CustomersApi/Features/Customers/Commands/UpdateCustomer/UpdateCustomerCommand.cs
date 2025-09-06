namespace Argo.VS.CustomersApi.Features.Customers.Commands.UpdateCustomer;

using Common;
using Infrastructure.CQRS;

public record UpdateCustomerCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string EmailAddress)
    : ICommand<CustomerResponse>;