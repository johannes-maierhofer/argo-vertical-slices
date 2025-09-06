namespace Argo.VS.CustomersApi.Infrastructure.Exceptions;

using System.Net;

using FluentValidation.Results;

public class ValidationException : CustomException
{
    private ValidationException()
        : base(HttpStatusCode.BadRequest, "One or more validation failures have occurred.")
    {
        this.Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        this.Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public IDictionary<string, string[]> Errors { get; }
}