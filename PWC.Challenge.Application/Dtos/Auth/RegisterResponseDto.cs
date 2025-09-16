namespace PWC.Challenge.Application.Dtos.Auth;

public record RegisterResponseDto(
    Guid UserId,
    Guid CustomerId,
    string Role
);
