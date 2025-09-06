using Argo.VS.CustomersApi.Features.Customers.Common;
using MediatR;

namespace Argo.VS.CustomersApi.Features.Customers.Commands.UpdateCustomer;

using Infrastructure.Web;

using Microsoft.AspNetCore.Mvc;

public class UpdateCustomerEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut($"{EndpointConfig.BaseApiPath}/customers", async (
                [FromBody] UpdateCustomerRequest request,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateCustomerCommand(
                    request.Id,
                    request.FirstName,
                    request.LastName,
                    request.EmailAddress);

                var result = await mediator.Send(command, cancellationToken);

                return Results.Ok(result);
            })
            .WithName("UpdateCustomer")
            .WithSummary("Update customer")
            .WithDescription("Update customer")
            .Produces<CustomerResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi()
            .WithApiVersionSet(builder.NewApiVersionSet("Customers").Build())
            .HasApiVersion(1.0);

        return builder;
    }
}

public record UpdateCustomerRequest(
    Guid Id,
    string FirstName,
    string LastName,
    string EmailAddress
);
