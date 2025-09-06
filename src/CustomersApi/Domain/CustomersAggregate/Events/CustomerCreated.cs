using Argo.VS.CustomersApi.Domain.Common.Events;

namespace Argo.VS.CustomersApi.Domain.CustomersAggregate.Events;

public record CustomerCreated(Customer Customer) : IDomainEvent;