namespace PWC.Challenge.Application.Dtos;

public record CarTypeRentalCountDto
{
    public string CarType { get; set; } = default!;
    public int RentalCount { get; set; } = default!;
    public decimal Percentage { get; set; } = default!;
}
