namespace PWC.Challenge.Application.Dtos;
public record AvailabilityQueryDto(
    DateTime PickingTime,
    DateTime ReturnDatte,
    string? CarType,      // opcional
    string? Model          // opcional
);
