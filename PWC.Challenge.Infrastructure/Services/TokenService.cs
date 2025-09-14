using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PWC.Challenge.Domain.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PWC.Challenge.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string accessToken, string refreshToken, DateTime expiration) GenerateTokens(Guid userId, string email, IList<string> roles)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["AccessTokenExpirationMinutes"]));

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = Guid.NewGuid().ToString(); // Simple refresh token for now

        return (accessToken, refreshToken, expires);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"];

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, // You might want to validate the audience for refresh tokens
            ValidateIssuer = false, // You might want to validate the issuer for refresh tokens
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateLifetime = false // Here we are validating an expired token
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    public TokenResult? GenerateAccessToken(Domain.Entities.Customer user, DateTime? expires = null)
    {
        throw new NotImplementedException();
    }

    public TokenResult? GenerateEmailVerificationToken(Domain.Entities.Customer user)
    {
        throw new NotImplementedException();
    }

    public TokenResult? GeneratePasswordResetToken(Domain.Entities.Customer user)
    {
        throw new NotImplementedException();
    }

    public (string Token, DateTime ExpiresAt) GenerateRefreshToken()
    {
        throw new NotImplementedException();
    }

    public (string Token, DateTime ExpiresAt) GenerateRefreshToken(DateTime expiresAt)
    {
        throw new NotImplementedException();
    }
}
