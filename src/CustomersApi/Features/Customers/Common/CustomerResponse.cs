namespace Argo.VS.CustomersApi.Features.Customers.Common;

public record CustomerResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string EmailAddress);
