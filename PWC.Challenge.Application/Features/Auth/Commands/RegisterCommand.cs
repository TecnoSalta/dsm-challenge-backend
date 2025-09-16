using MediatR;
using PWC.Challenge.Application.Dtos.Auth;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Application.Features.Auth.Commands;

public record RegisterCommand(RegisterRequestDto RegisterRequest) : IRequest<ApplicationUser>;
