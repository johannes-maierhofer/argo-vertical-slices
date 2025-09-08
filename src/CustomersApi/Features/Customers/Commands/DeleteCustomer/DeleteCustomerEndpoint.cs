namespace Argo.VS.CustomersApi.Features.Customers.Commands.DeleteCustomer;

using Infrastructure.Web;

using MediatR;

using Microsoft.AspNetCore.Mvc;

public class DeleteCustomerEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{EndpointConfig.BaseApiPath}/customers/{{id:guid}}", async (
                [FromRoute] Guid id,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new DeleteCustomerCommand(id);

                await mediator.Send(command, cancellationToken);

                return Results.NoContent();
            })
            .WithName("DeleteCustomer")
            .WithSummary("Delete customer")
            .WithDescription("Delete a customer by ID")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .WithApiVersionSet(builder.NewApiVersionSet("Customers").Build())
            .HasApiVersion(1.0);

        return builder;
    }
}
