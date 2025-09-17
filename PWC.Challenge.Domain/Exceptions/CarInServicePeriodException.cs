using PWC.Challenge.Domain.Exceptions;

namespace PWC.Challenge.Domain.Exceptions;

public class CarInServicePeriodException : DomainException
{
    public CarInServicePeriodException(Guid carId, DateOnly startDate, DateOnly endDate)
        : base($"Car {carId} has a service scheduled that overlaps with the period from {startDate} to {endDate}.")
    {
    }
}
