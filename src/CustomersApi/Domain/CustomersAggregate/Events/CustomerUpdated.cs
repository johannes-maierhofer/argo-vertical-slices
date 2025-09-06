using Argo.VS.CustomersApi.Domain.Common.Events;

namespace Argo.VS.CustomersApi.Domain.CustomersAggregate.Events;

public record CustomerUpdated(Customer Customer) : IDomainEvent;
