namespace PWC.Challenge.Application.Dtos.Auth;

public class RegisterRequestDto
{
    public string Dni { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Address { get; set; } = null!;
}

