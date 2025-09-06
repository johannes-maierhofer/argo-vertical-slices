namespace Argo.VS.CustomersApi.Infrastructure.Web;

using Exceptions;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

public static class ProblemDetailsExtensions
{
    public static WebApplication UseCustomProblemDetails(this WebApplication app)
    {
        app.UseStatusCodePages(statusCodeHandlerApp =>
        {
            statusCodeHandlerApp.Run(async context =>
            {
                context.Response.ContentType = "application/problem+json";

                if (context.RequestServices.GetService<IProblemDetailsService>() is { } problemDetailsService)
                {
                    await problemDetailsService.WriteAsync(new ProblemDetailsContext
                    {
                        HttpContext = context,
                        ProblemDetails =
                        {
                            Detail = ReasonPhrases.GetReasonPhrase(context.Response.StatusCode),
                            Status = context.Response.StatusCode
                        }
                    });
                }
            });
        });

        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                context.Response.ContentType = "application/problem+json";

                if (context.RequestServices.GetService<IProblemDetailsService>() is { } problemDetailsService)
                {
                    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exceptionType = exceptionHandlerFeature?.Error;

                    if (exceptionType is not null)
                    {
                        (string Detail, string Title, int StatusCode) details = exceptionType switch
                        {
                            CustomException customException =>
                            (
                                exceptionType.Message,
                                exceptionType.GetType().Name,
                                context.Response.StatusCode = (int)customException.StatusCode
                            ),
                            DbUpdateConcurrencyException =>
                            (
                                exceptionType.Message,
                                exceptionType.GetType().Name,
                                context.Response.StatusCode = StatusCodes.Status409Conflict
                            ),
                            _ =>
                            (
                                exceptionType.Message,
                                exceptionType.GetType().Name,
                                context.Response.StatusCode = StatusCodes.Status500InternalServerError
                            )
                        };

                        var problem = new ProblemDetailsContext
                        {
                            HttpContext = context,
                            ProblemDetails =
                            {
                                Title = details.Title, Detail = details.Detail, Status = details.StatusCode
                            }
                        };

                        // add validation error details
                        if (exceptionHandlerFeature?.Error is ValidationException validationException)
                        {
                            problem.ProblemDetails.Extensions.Add("errors", validationException.Errors);
                        }

                        // show exception details in dev mode
                        if (app.Environment.IsDevelopment())
                        {
                            problem.ProblemDetails.Extensions.Add("exception",
                                exceptionHandlerFeature?.Error?.ToString());
                        }

                        await problemDetailsService.WriteAsync(problem);
                    }
                }
            });
        });

        return app;
    }
}
