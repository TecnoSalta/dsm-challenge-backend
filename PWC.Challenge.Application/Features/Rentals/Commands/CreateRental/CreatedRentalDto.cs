using System;

namespace PWC.Challenge.Application.Features.Rentals.Commands.CreateRental;

public class CreatedRentalDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid CarId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Status { get; set; } = null!;
}