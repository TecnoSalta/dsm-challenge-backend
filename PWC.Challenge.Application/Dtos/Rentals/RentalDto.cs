using PWC.Challenge.Domain.Enums;

namespace PWC.Challenge.Application.Dtos.Rentals;

public class RentalDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid CarId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public RentalStatus Status { get; set; }
    public decimal DailyRate { get; set; }
    public decimal TotalCost { get; set; }
    public DateOnly? ActualReturnDate { get; set; }
}
