using MediatR;
using PWC.Challenge.Application.Services;

namespace PWC.Challenge.Application.Features.Rentals.Commands.CreateRental;

public class CreateRentalCommandHandler(
    IRentalService rentalService) : IRequestHandler<CreateRentalCommand, CreatedRentalDto>
{
    public async Task<CreatedRentalDto> Handle(CreateRentalCommand command, CancellationToken cancellationToken)
    {
        var rental = await rentalService.RegisterRentalAsync(command.CreateDto);

        return new CreatedRentalDto
        {
            Id = rental.Id,
            CarId = rental.Car.Id,
            CustomerId = rental.Customer.Id,
            StartDate = rental.RentalPeriod.StartDate,
            EndDate = rental.RentalPeriod.EndDate,
            Status = rental.Status.ToString(),
        };
    }
}