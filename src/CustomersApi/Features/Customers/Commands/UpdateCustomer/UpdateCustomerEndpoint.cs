using Argo.VS.CustomersApi.Features.Customers.Common;
using MediatR;

namespace Argo.VS.CustomersApi.Features.Customers.Commands.UpdateCustomer;

using Infrastructure.Web;

using Microsoft.AspNetCore.Mvc;

public class UpdateCustomerEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut($"{EndpointConfig.BaseApiPath}/customers/{{id:guid}}", async (
                [FromRoute] Guid id,
                [FromBody] UpdateCustomerRequest request,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateCustomerCommand(
                    id,
                    request.FirstName,
                    request.LastName,
                    request.EmailAddress);

                var result = await mediator.Send(command, cancellationToken);

                return Results.Ok(result);
            })
            .WithName("UpdateCustomer")
            .WithSummary("Update customer")
            .WithDescription("Update a customer by ID")
            .Produces<CustomerResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi()
            .WithApiVersionSet(builder.NewApiVersionSet("Customers").Build())
            .HasApiVersion(1.0);

        return builder;
    }
}

public record UpdateCustomerRequest(
    string FirstName,
    string LastName,
    string EmailAddress
);
