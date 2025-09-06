namespace Argo.VS.CustomersApi.Infrastructure.CQRS;

using MediatR;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand
{
    new Task Handle(TCommand command, CancellationToken cancellationToken);
}

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : notnull
{
    new Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken);
}
