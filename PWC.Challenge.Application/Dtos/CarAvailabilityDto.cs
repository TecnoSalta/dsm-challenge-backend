namespace PWC.Challenge.Application.Dtos;

public record CarAvailabilityDto(
    Guid Id,
    string Type,
    string Model,
    decimal DailyRate,
    string LicensePlate,
    string Status,
    bool IsAvailableForPeriod
);