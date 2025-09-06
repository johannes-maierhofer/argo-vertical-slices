namespace Argo.VS.CustomersApi.Infrastructure.CQRS;

using MediatR;

public interface IQuery<out T> : IRequest<T>;
