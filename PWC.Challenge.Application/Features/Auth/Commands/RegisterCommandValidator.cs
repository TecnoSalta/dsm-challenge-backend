using FluentValidation;

namespace PWC.Challenge.Application.Features.Auth.Commands;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.RegisterRequest.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.RegisterRequest.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.RegisterRequest.FirstName)
            .NotEmpty().WithMessage("First name is required.");

        RuleFor(x => x.RegisterRequest.LastName)
            .NotEmpty().WithMessage("Last name is required.");
    }
}
