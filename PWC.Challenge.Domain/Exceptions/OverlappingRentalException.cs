namespace PWC.Challenge.Domain.Exceptions;

public class OverlappingRentalException : DomainException
{
    public OverlappingRentalException()
    : base("The car already has an active rental that overlaps with the specified period.")
    {
    }
}