using MediatR;
using PWC.Challenge.Application.Dtos.Auth;

namespace PWC.Challenge.Application.Features.Auth.Commands;

public record RegisterCommand(RegisterRequestDto RegisterRequest) : IRequest<AuthResponseDto>;
