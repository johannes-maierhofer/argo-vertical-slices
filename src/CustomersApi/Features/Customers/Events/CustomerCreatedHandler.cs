using MediatR;

namespace Argo.VS.CustomersApi.Features.Customers.Events;

using Domain.CustomersAggregate.Events;

public class CustomerCreatedHandler : INotificationHandler<CustomerCreated>
{
    public Task Handle(CustomerCreated @event, CancellationToken cancellationToken)
    {
        // Do something. E.g. publish message to broker
        return Task.CompletedTask;
    }
}