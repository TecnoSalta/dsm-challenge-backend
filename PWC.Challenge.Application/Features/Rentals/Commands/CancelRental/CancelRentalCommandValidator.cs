using FluentValidation;

namespace PWC.Challenge.Application.Features.Rentals.Commands.CancelRental;

public sealed class CancelRentalCommandValidator : AbstractValidator<CancelRentalCommand>
{
    public CancelRentalCommandValidator()
    {
        RuleFor(x => x.RentalId).NotEmpty();
    }
}