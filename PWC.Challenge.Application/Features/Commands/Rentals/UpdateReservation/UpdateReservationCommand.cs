using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Common.CQRS;

namespace PWC.Challenge.Application.Features.Commands.Rentals.UpdateReservation;

public record UpdateReservationCommand(
    Guid ReservationId,
    UpdateReservationDto Payload
) : ICommand<UpdatedReservationDto>;