namespace PWC.Challenge.Application.Dtos;

public record AvailableCarDto(
    Guid Id,
    string Type,
    string Model,
    decimal DailyRate
);