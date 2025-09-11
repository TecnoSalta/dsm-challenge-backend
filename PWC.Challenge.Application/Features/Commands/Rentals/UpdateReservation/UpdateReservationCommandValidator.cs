using FluentValidation;

namespace PWC.Challenge.Application.Features.Commands.Rentals.UpdateReservation;

public sealed class UpdateReservationCommandValidator : AbstractValidator<UpdateReservationCommand>
{
    public UpdateReservationCommandValidator()
    {
        RuleFor(x => x.ReservationId).NotEmpty();
        RuleFor(x => x.Payload).NotNull();

        When(x => x.Payload.NewStartDate.HasValue, () =>
            RuleFor(x => x.Payload.NewStartDate.Value)
                .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("New start date must be in the future."));

        When(x => x.Payload.NewEndDate.HasValue, () =>
            RuleFor(x => x.Payload.NewEndDate.Value)
                .GreaterThan(x => x.Payload.NewStartDate ?? DateOnly.MinValue)
                .WithMessage("New end date must be after start date."));
    }
}