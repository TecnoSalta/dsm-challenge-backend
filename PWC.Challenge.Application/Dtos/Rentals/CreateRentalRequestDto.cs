namespace PWC.Challenge.Application.Dtos.Rentals;

public record CreateRentalRequestDto(
    Guid CustomerId,
    Guid CarId,
    DateOnly StartDate,
    DateOnly EndDate
);