using FluentValidation;

namespace PWC.Challenge.Application.Features.Customers.Commands.CreateCustomer
{
    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {

            RuleFor(c => c.Customer.FullName)
                .NotEmpty()
                    .WithMessage("FullName is required.")
                    .WithState(_ => "errors.fullNameRequired");

            RuleFor(c => c.Customer.Address)
                .NotEmpty()
                    .WithMessage("Address is required.")
                    .WithState(_ => "errors.addressRequired");
        }
    }
}
