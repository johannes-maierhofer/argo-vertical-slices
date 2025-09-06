namespace Argo.VS.CustomersApi.Features.Customers.Commands.CreateCustomer;

using Common;
using Infrastructure.CQRS;

public record CreateCustomerCommand(
    string FirstName,
    string LastName,
    string EmailAddress)
    : ICommand<CustomerResponse>;
