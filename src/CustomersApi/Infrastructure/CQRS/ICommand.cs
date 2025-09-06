namespace Argo.VS.CustomersApi.Infrastructure.CQRS;

public interface ICommand<out TResponse> : IRequest<TResponse>;
