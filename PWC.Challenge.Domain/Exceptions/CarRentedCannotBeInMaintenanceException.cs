namespace PWC.Challenge.Domain.Exceptions;

public class CarRentedCannotBeInMaintenanceException : DomainException
{
    public CarRentedCannotBeInMaintenanceException() 
        : base("Cannot mark a rented car as in maintenance.")
    {
    }
}
