namespace Argo.VS.CustomersApi.Infrastructure.CQRS.Behaviors;

using CQRS;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Persistence;

public class TransactionBehavior<TRequest, TResponse>(CustomerDbContext dbContext)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // for Connection Resiliency
        // see https://learn.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
        if (dbContext.Database.CurrentTransaction is not null)
        {
            var responseInAmbientTransaction = await next(cancellationToken);

            // Commit unit-of-work here so EF/Consumer Outbox persist changes & messages.
            await dbContext.SaveChangesAsync(cancellationToken);
            return responseInAmbientTransaction;
        }

        var response = default(TResponse);

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(
            async () =>
            {
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

                response = await next(cancellationToken);

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            });

        return response!;
    }
}
