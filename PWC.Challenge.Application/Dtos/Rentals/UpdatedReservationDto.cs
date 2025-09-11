namespace PWC.Challenge.Application.Dtos.Rentals;

/// <summary>
//datos de la reserva después de la modificación
/// </summary>
public record UpdatedReservationDto(
    Guid ReservationId,
    Guid CarId,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    string Message
);