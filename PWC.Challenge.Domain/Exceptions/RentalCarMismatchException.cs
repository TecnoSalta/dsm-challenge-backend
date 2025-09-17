namespace PWC.Challenge.Domain.Exceptions;

public class RentalCarMismatchException : DomainException
{
    public RentalCarMismatchException() 
        : base("Rental does not belong to this car.")
    {
    }
}
