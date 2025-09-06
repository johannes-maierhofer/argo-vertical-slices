using Argo.VS.CustomersApi.Features.Customers.Common;
using MediatR;

namespace Argo.VS.CustomersApi.Features.Customers.Commands.CreateCustomer;

using Infrastructure.Web;

using Microsoft.AspNetCore.Mvc;

public class CreateCustomerEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/customers", async (
                [FromBody] CreateCustomerRequest request,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateCustomerCommand(
                    request.FirstName,
                    request.LastName,
                    request.EmailAddress);

                var result = await mediator.Send(command, cancellationToken);

                return Results.Ok(result);
            })
            .WithName("CreateCustomer")
            .WithSummary("Create customer")
            .WithDescription("Create customer")
            .Produces<CustomerResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi()
            .WithApiVersionSet(builder.NewApiVersionSet("Customers").Build())
            .HasApiVersion(1.0);

        return builder;
    }
}

public record CreateCustomerRequest(
    string FirstName,
    string LastName,
    string EmailAddress
);
