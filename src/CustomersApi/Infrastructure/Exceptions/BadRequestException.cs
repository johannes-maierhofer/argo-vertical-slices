namespace Argo.VS.CustomersApi.Infrastructure.Exceptions;

using System.Net;

public class BadRequestException(
    string message) : CustomException(HttpStatusCode.BadRequest, message);
