using PWC.Challenge.Application.Dtos.Rentals;

namespace PWC.Challenge.Application.Features.Rentals.Commands.UpdateRental.Services;

public interface IRentalService
{
    Task<UpdatedRentalDto> UpdateReservationAsync(
        Guid rentalId,
        DateOnly? newStart,
        DateOnly? newEnd,
        Guid? newCarId,
        CancellationToken ct);
}
