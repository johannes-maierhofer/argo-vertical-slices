using MediatR;

namespace Argo.VS.CustomersApi.Features.Customers.Events;

using Domain.CustomersAggregate.Events;

public class CustomerUpdatedHandler : INotificationHandler<CustomerUpdated>
{
    public Task Handle(CustomerUpdated @event, CancellationToken cancellationToken)
    {
        // Do something. E.g. publish message to broker
        return Task.CompletedTask;
    }
}