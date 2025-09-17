namespace PWC.Challenge.Domain.Exceptions;

public class CarNotAvailableException : DomainException
{
    public CarNotAvailableException(Guid carId) 
        : base($"Car with ID '{carId}' is not available to rent.")
    {
    }
}
