using MediatR;
using PWC.Challenge.Application.Dtos.Auth;

namespace PWC.Challenge.Application.Features.Auth.Commands;

public record LoginCommand(string Email, Guid UserId, List<string> Roles, Guid? CustomerId) : IRequest<AuthResponseDto>;
