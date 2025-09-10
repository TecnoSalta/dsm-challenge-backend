namespace PWC.Challenge.Application.Dtos;

public record AvailableCarDto(
    Guid CarId,
    string Type,
    string Model
);