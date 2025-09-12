using FluentValidation;

namespace PWC.Challenge.Application.Features.Rentals.Commands.CompleteRental;

public sealed class CompleteRentalCommandValidator : AbstractValidator<CompleteRentalCommand>
{
    public CompleteRentalCommandValidator()
    {
        RuleFor(x => x.RentalId).NotEmpty();
    }
}