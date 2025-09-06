namespace Argo.VS.CustomersApi.Infrastructure.CQRS;

public interface IQuery<out TResponse> : IRequest<TResponse>;
