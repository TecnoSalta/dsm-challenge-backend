namespace PWC.Challenge.Application.Dtos.Rentals;

/// <summary>
//datos de la reserva después de la modificación
/// </summary>
public record UpdatedRentalDto(
    Guid RentalId,
    Guid CarId,
    DateOnly StartDate,
    DateOnly EndDate,
    string Message
);