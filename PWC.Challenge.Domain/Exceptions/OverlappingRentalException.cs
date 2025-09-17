using System;

namespace PWC.Challenge.Domain.Exceptions;

public class OverlappingRentalException : DomainException
{
    public OverlappingRentalException(Guid carId, DateOnly startDate, DateOnly endDate)
    : base($"Car {carId} has an overlapping rental for the period from {startDate} to {endDate}.")
    {
    }

    public OverlappingRentalException()
    : base("The car already has an active rental that overlaps with the specified period.")
    {
    }
}
