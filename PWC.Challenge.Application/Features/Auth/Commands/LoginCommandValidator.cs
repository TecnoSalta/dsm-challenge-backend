using FluentValidation;

namespace PWC.Challenge.Application.Features.Auth.Commands;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.LoginRequest.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.LoginRequest.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
