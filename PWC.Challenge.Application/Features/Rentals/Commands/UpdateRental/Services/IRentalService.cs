using PWC.Challenge.Application.Dtos.Rentals;

namespace PWC.Challenge.Application.Features.Rentals.Commands.UpdateRental.Services;

public interface IRentalService
{
    Task<UpdatedRentalDto> UpdateRentalAsync(
        Guid rentalId,
        DateOnly? newStart,
        DateOnly? newEnd,
        Guid? newCarId,
        CancellationToken ct);

    Task<CancelledRentalDto> CancelRentalAsync(
      Guid rentalId,
      CancellationToken ct);
}
