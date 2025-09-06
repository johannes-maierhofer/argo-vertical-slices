namespace Argo.VS.CustomersApi.Infrastructure.Persistence.Interceptors;

using Domain.Common;
using Domain.Common.Events;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class DomainEventDispatchingInterceptor(
    IDomainEventPublisher domainEventPublisher) : SaveChangesInterceptor
{
    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        return this.SavedChangesAsync(eventData, result)
            .GetAwaiter()
            .GetResult();
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = new())
    {
        if (eventData.Context == null)
        {
            return result;
        }

        var dbContext = eventData.Context;
        await this.DispatchDomainEvents(dbContext);

        return result;
    }

    private async Task DispatchDomainEvents(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        var entities = context.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities
            .ToList()
            .ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await domainEventPublisher.Publish(domainEvent);
        }
    }
}
