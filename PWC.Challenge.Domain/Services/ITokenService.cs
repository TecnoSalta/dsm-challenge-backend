using PWC.Challenge.Domain.Entities;
using System.Security.Claims;

public interface ITokenService
{
    (string accessToken, string refreshToken, DateTime expiration) GenerateTokens(Guid userId, string email, IList<string> roles, Guid? customerId);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    TokenResult? GenerateAccessToken(Customer user, DateTime? expires = null);
    TokenResult? GenerateEmailVerificationToken(Customer user);
    TokenResult? GeneratePasswordResetToken(Customer user);
    (string Token, DateTime ExpiresAt) GenerateRefreshToken();
    (string Token, DateTime ExpiresAt) GenerateRefreshToken(DateTime expiresAt);
}

public record TokenResult(string AccessToken, DateTime Expiration);

public static class TokenClaimTypes
{
    public const string FullName = "FullName";
}