using MediatR;
using Microsoft.AspNetCore.Identity;
using PWC.Challenge.Application.Dtos.Auth;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Services;
using PWC.Challenge.Common.Exceptions;

namespace PWC.Challenge.Application.Features.Auth.Commands;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;

    public RefreshTokenCommandHandler(ITokenService tokenService, UserManager<ApplicationUser> userManager)
    {
        _tokenService = tokenService;
        _userManager = userManager;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        var userId = Guid.Parse(principal.Identity.Name); // Assuming Name claim stores UserId
        var customerIdClaim = principal.FindFirst("customerId");
        Guid? customerId = null;
        if (customerIdClaim != null)
        {
            customerId = Guid.Parse(customerIdClaim.Value);
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new BadRequestException("Invalid client request or refresh token expired.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var (newAccessToken, newRefreshToken, newExpiration) = _tokenService.GenerateTokens(user.Id, user.Email, roles, customerId);

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken,
            Expiration = newExpiration
        };
    }
}
