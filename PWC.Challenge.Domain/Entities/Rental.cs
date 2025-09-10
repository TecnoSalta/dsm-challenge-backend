using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Domain.Rentals;

public class Rental
{
    public Guid Id { get; private set; }
    public Customer Customer { get; private set; }
    public Car Car { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }

    public Rental(Guid id, Customer customer, Car car, DateOnly startDate, DateOnly endDate)
    {
        Id = id;
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        Car = car ?? throw new ArgumentNullException(nameof(car));
        StartDate = startDate;
        EndDate = endDate;
    }
}