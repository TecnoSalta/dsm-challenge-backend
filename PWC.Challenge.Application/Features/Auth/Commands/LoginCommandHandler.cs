using MediatR;
using Microsoft.AspNetCore.Identity;
using PWC.Challenge.Application.Dtos.Auth;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Services;
using PWC.Challenge.Common.Exceptions;

namespace PWC.Challenge.Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.LoginRequest.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.LoginRequest.Password))
        {
            throw new BadRequestException("Invalid credentials.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var (accessToken, refreshToken, expiration) = _tokenService.GenerateTokens(user.Id, user.Email, roles);

        return new AuthResponseDto
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            Expiration = expiration
        };
    }
}
