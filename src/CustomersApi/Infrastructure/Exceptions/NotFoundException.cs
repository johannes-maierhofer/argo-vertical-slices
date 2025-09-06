namespace Argo.VS.CustomersApi.Infrastructure.Exceptions;

using System.Net;

public class NotFoundException(
    string message)
    : CustomException(statusCode: HttpStatusCode.NotFound, message);