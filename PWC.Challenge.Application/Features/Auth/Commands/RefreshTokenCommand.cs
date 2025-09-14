using MediatR;
using PWC.Challenge.Application.Dtos.Auth;

namespace PWC.Challenge.Application.Features.Auth.Commands;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<AuthResponseDto>;
