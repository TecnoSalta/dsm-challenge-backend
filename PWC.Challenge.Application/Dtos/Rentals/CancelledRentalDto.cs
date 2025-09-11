namespace PWC.Challenge.Application.Dtos.Rentals;

/// <summary>
//datos de la reserva después de la cancelación
/// </summary>
public record CancelledRentalDto(
    Guid RentalId,
    string Message);