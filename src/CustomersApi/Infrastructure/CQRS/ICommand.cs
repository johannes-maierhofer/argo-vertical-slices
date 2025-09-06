namespace Argo.VS.CustomersApi.Infrastructure.CQRS;

using MediatR;

public interface ICommand : IRequest, IBaseCommand;

public interface ICommand<out TResponse> : IRequest<TResponse>, IBaseCommand
    where TResponse : notnull;

public interface IBaseCommand;
