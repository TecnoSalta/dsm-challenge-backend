namespace PWC.Challenge.Application.Dtos.Rentals;

/// <summary
/// datos que el cliente puede modificar
/// </summary>
public record UpdateReservationDto(
    DateOnly? NewStartDate,
    DateOnly? NewEndDate,
    Guid? NewCarId
);