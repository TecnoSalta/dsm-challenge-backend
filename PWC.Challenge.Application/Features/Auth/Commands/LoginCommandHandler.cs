using MediatR;
using Microsoft.AspNetCore.Identity;
using PWC.Challenge.Application.Dtos.Auth;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Services;
using PWC.Challenge.Common.Exceptions;

namespace PWC.Challenge.Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var (accessToken, refreshToken, expiration) = _tokenService.GenerateTokens(request.UserId, request.Email, request.Roles, request.CustomerId);

        return new AuthResponseDto
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            Expiration = expiration,
            Role = request.Roles.FirstOrDefault(), // Assuming single role for simplicity
            CustomerId = request.CustomerId
        };
    }
}
