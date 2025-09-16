namespace PWC.Challenge.Application.Dtos.Profile;

public class ProfileDto
{
    public Guid UserId { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public ProfileCustomerDto? Customer { get; set; }
}
