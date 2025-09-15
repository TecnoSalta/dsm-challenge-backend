using System;
using System.ComponentModel.DataAnnotations;

namespace PWC.Challenge.Application.Features.Rentals.Commands.CreateRental;

public class CreateRentalDto
{
    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    public Guid CarId { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }
}