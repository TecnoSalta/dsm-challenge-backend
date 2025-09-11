using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Common.CQRS;

namespace PWC.Challenge.Application.Features.Rentals.Commands.CancelRental;

public record CancelRentalCommand(
    Guid ReservationId,
    UpdateRentalDto Payload
) : ICommand<UpdatedRentalDto>;