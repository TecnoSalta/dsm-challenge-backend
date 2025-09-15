using FluentValidation;

namespace PWC.Challenge.Application.Features.Rentals.Commands.CompleteRental;

public sealed class CompleteRentalCommandValidator : AbstractValidator<CompleteRentalCommand>
{
    public CompleteRentalCommandValidator()
    {
        RuleFor(x => x.RouteId).NotEmpty();
        RuleFor(x => x.BodyRentalId).NotEmpty();
        RuleFor(x => x.RouteId)
            .Equal(x => x.BodyRentalId)
            .WithMessage("ID de URL no coincide con ID del cuerpo.");
    }
}