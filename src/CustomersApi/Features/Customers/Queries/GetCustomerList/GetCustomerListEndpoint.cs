namespace Argo.VS.CustomersApi.Features.Customers.Queries.GetCustomerList;

using Common;

using Infrastructure.Web;

using MediatR;

using Microsoft.AspNetCore.Mvc;

public class GetCustomerListEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/customers", async (
                [FromQuery] int pageNumber,
                [FromQuery] int pageSize,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new GetCustomerListQuery(pageNumber, pageSize),
                    cancellationToken);

                return Results.Ok(result);
            })
            .WithName("GetCustomerList")
            .WithSummary("Get customer List")
            .WithDescription("Get customer List")
            .Produces<CustomerListResponse>()
            .WithOpenApi()
            .WithApiVersionSet(builder.NewApiVersionSet("Customers").Build())
            .HasApiVersion(1.0);

        return builder;
    }
}
