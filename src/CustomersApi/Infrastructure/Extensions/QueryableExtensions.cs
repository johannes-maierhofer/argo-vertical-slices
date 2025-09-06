namespace Argo.VS.CustomersApi.Infrastructure.Extensions;

using Models;

using Microsoft.EntityFrameworkCore;

public static class QueryableExtensions
{
    public static Task<PaginatedList<TDestination>> ToPaginatedListAsync<TDestination>(
        this IQueryable<TDestination> queryable,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    ) where TDestination : class
    {
        return PaginatedList<TDestination>.CreateAsync(
            queryable.AsNoTracking(),
            pageNumber,
            pageSize,
            cancellationToken);
    }
}
