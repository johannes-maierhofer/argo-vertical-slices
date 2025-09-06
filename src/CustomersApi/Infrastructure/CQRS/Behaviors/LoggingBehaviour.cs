namespace Argo.VS.CustomersApi.Infrastructure.CQRS.Behaviors;

using MediatR;

using Microsoft.Extensions.Logging;

public class LoggingBehaviour<TRequest, TResponse>(
    ILogger<TRequest> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : notnull
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation("Handle Request Name={Name} with Request={@Request}",
            requestName,
            request);

        return next(cancellationToken);
    }
}