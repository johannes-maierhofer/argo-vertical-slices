namespace Argo.VS.CustomersApi.Features.Customers.Queries.GetCustomer;

using Common;

using Infrastructure.Web;

using MediatR;

using Microsoft.AspNetCore.Mvc;

public class GetCustomerEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/customers/{{id}}", async (
                [FromRoute] Guid id,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new GetCustomerQuery(id), cancellationToken);
                return Results.Ok(result);
            })
            .WithName("GetCustomer")
            .WithSummary("Get customer")
            .WithDescription("Get customer")
            .Produces<CustomerResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .WithApiVersionSet(builder.NewApiVersionSet("Customers").Build())
            .HasApiVersion(1.0);

        return builder;
    }
}
