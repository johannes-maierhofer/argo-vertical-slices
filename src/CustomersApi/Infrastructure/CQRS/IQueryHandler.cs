namespace Argo.VS.CustomersApi.Infrastructure.CQRS;

using MediatR;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    new Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
}
