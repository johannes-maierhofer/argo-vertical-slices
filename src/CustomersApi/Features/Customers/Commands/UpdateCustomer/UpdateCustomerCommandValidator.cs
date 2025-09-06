namespace Argo.VS.CustomersApi.Features.Customers.Commands.UpdateCustomer;

using FluentValidation;

using Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    private readonly CustomerDbContext _dbContext;

    public UpdateCustomerCommandValidator(CustomerDbContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name may not be empty");
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name may not be empty");
        RuleFor(x => x.EmailAddress)
            .NotEmpty().WithMessage("Email may not be empty")
            .EmailAddress().WithMessage("Email address is invalid");
        RuleFor(x => x)
            .MustAsync(BeUniqueEmail).WithMessage("A customer with the specified email already exists.");
    }

    private async Task<bool> BeUniqueEmail(UpdateCustomerCommand command, CancellationToken cancellationToken)
    {
        var alreadyExists = await _dbContext.Customers
            .AnyAsync(c => c.Id != command.Id &&  c.EmailAddress == command.EmailAddress, cancellationToken);

        return !alreadyExists;
    }
}
