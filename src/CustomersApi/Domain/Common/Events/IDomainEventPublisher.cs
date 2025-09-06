namespace Argo.VS.CustomersApi.Domain.Common.Events;

public interface IDomainEventPublisher
{
    Task Publish(IDomainEvent domainEvent);
}