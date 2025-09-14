namespace PWC.Challenge.Application.Dtos.Auth;

public class AuthResponseDto
{
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime Expiration { get; set; }
}
