using Microsoft.AspNetCore.Http;
using PWC.Challenge.Common.Exceptions;
using PWC.Challenge.Domain.Services;
using System.Security.Claims;

namespace PWC.Challenge.Application.Services;

public class AuthService(
    IHttpContextAccessor httpContextAccessor) : IAuthService
{
    public bool IsAuthenticated()
    {
        var isAuthenticated = httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        return isAuthenticated;
    }

    public string GetFullName()
    {
        var firstName = httpContextAccessor.HttpContext?.User.Claims?
            .FirstOrDefault(e => e.Type == TokenClaimTypes.FullName)?.Value ?? throw new InternalServerException($"ClaimType \"{TokenClaimTypes.FullName}\" not found.");
        
        return firstName;
    }

    public Guid GetUserId()
    {
        var nameIdentifier = httpContextAccessor.HttpContext?.User.Claims?
            .FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new InternalServerException($"ClaimType \"{ClaimTypes.NameIdentifier}\" not found.");

        if (!Guid.TryParse(nameIdentifier, out var userGuid))
            throw new InternalServerException($"NameIdentifier claim value isn't valid Guid value.");

        return userGuid;
    }
}
