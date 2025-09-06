using MediatR;

namespace Argo.VS.CustomersApi.Domain.Common.Events;

public class DomainEventPublisher(IMediator mediator) : IDomainEventPublisher
{
    public async Task Publish(IDomainEvent domainEvent)
    {
        await mediator.Publish(domainEvent);
    }
}