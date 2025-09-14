using MediatR;
using Microsoft.AspNetCore.Identity;
using PWC.Challenge.Application.Dtos.Auth;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Services;
using PWC.Challenge.Common.Exceptions;

namespace PWC.Challenge.Application.Features.Auth.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public RegisterCommandHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.RegisterRequest.Email);
        if (existingUser != null)
        {
            throw new BadRequestException("User with this email already exists.");
        }

        var user = new ApplicationUser
        {
            Email = request.RegisterRequest.Email,
            UserName = request.RegisterRequest.Email,
            FirstName = request.RegisterRequest.FirstName,
            LastName = request.RegisterRequest.LastName
        };

        var result = await _userManager.CreateAsync(user, request.RegisterRequest.Password);
        if (!result.Succeeded)
        {
            throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // Assign default role, e.g., "Customer"
        await _userManager.AddToRoleAsync(user, "Customer");

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
