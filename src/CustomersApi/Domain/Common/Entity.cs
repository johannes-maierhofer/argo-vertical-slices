using System.ComponentModel.DataAnnotations.Schema;
using Argo.VS.CustomersApi.Domain.Common.Events;

namespace Argo.VS.CustomersApi.Domain.Common;

public abstract class Entity<TId> : IHasDomainEvents
    where TId : struct
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public TId Id { get; protected init; }
        
    [NotMapped]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}