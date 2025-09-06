namespace Argo.VS.CustomersApi.Infrastructure.Exceptions;

using System.Net;

public class CustomException : Exception
{
    protected CustomException(
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        string message = "",
        Exception? innerException = null)
        : base(message, innerException)
    {
        this.StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; }
}
