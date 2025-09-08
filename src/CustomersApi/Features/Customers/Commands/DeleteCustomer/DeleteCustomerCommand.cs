namespace Argo.VS.CustomersApi.Features.Customers.Commands.DeleteCustomer;

using Infrastructure.CQRS;

public record DeleteCustomerCommand(Guid Id)
    : ICommand;
