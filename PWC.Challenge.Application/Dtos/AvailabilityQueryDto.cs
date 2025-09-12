namespace PWC.Challenge.Application.Dtos;

public record AvailabilityQueryDto(
    DateOnly StartDate,
    DateOnly EndDate,
    string? CarType = null,
    string? Model = null
);