using FluentValidation;

namespace PWC.Challenge.Application.Features.Rentals.Commands.CreateRental;

public class CreateRentalCommandValidator : AbstractValidator<CreateRentalCommand>
{
    public CreateRentalCommandValidator()
    {
        RuleFor(x => x.CreateDto.CustomerId).NotEmpty().WithMessage("Customer ID is required.");
        RuleFor(x => x.CreateDto.CarId).NotEmpty().WithMessage("Car ID is required.");
        RuleFor(x => x.CreateDto.StartDate).NotEmpty().WithMessage("Start date is required.");
        RuleFor(x => x.CreateDto.EndDate).NotEmpty().WithMessage("End date is required.");

        RuleFor(x => x.CreateDto.StartDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Start date cannot be in the past.");

        RuleFor(x => x.CreateDto.EndDate)
            .GreaterThan(x => x.CreateDto.StartDate)
            .WithMessage("End date must be after start date.");
    }
}