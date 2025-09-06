using Argo.VS.CustomersApi.Domain.Common;
using Argo.VS.CustomersApi.Domain.CustomersAggregate.Events;

namespace Argo.VS.CustomersApi.Domain.CustomersAggregate;

public class Customer : Entity<Guid>
{
    private Customer(
        Guid id,
        string firstName,
        string lastName,
        string emailAddress)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
    }

    private Customer()
    {
        // EF Core
    }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string EmailAddress { get; private set; }

    public static Customer Create(
        string firstName,
        string lastName,
        string emailAddress)
    {
        var customer = new Customer(
            Guid.NewGuid(),
            firstName,
            lastName,
            emailAddress);

        customer.AddDomainEvent(new CustomerCreated(customer));

        return customer;
    }

    public void Update(
        string firstName,
        string lastName,
        string emailAddress)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
        this.EmailAddress = emailAddress;

        AddDomainEvent(new CustomerUpdated(this));
    }
}