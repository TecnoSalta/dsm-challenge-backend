namespace PWC.Challenge.Application.Dtos.Rentals;

/// <summary
/// datos que el cliente puede modificar
/// </summary>
public record UpdateRentalDto(
    DateOnly? NewStartDate,
    DateOnly? NewEndDate,
    Guid? NewCarId
);