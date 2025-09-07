namespace Argo.VS.CustomersApi.IntegrationTests.Testing.Builders;

using Domain.CustomersAggregate;

public class CustomerBuilder
{
    private string _emailAddress = $"user-{Guid.NewGuid():N}@example.test";
    private string _firstName = "Ada";
    private string _lastName = "Lovelace";

    public CustomerBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public CustomerBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public CustomerBuilder WithEmailAddress(string emailAddress)
    {
        _emailAddress = emailAddress;
        return this;
    }

    public Customer Build()
    {
        return Customer.Create(_firstName, _lastName, _emailAddress);
    }
}
