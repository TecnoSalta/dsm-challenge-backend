using PWC.Challenge.Domain.Enums;

namespace PWC.Challenge.Application.Dtos;

public class CarDto
{
    public Guid Id { get; set; }
    public string? Type { get; set; }
    public string Model { get; set; }
    public decimal DailyRate { get; set; }
    public string LicensePlate { get; set; } = default!; // Added this line
    public List<ServiceDto> Services { get; set; } = new List<ServiceDto>();
}