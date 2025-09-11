using PWC.Challenge.Application.Dtos.Rentals;

namespace PWC.Challenge.Application.Features.Commands.Rentals.UpdateReservation.Services;

public interface IRentalService
{
    Task<UpdatedReservationDto> UpdateReservationAsync(
        Guid rentalId,
        DateOnly? newStart,
        DateOnly? newEnd,
        Guid? newCarId,
        CancellationToken ct);
}
