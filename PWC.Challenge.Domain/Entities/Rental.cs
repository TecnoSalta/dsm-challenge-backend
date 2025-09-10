namespace PWC.Challenge.Domain.Entities;

public class Rental(Guid id, Customer customer, Car car, DateOnly startDate, DateOnly endDate)
{
    public Guid Id { get; private set; } = id;
    public Customer Customer { get; private set; } = customer ?? throw new ArgumentNullException(nameof(customer));
    public Car Car { get; private set; } = car ?? throw new ArgumentNullException(nameof(car));
    public DateOnly StartDate { get; private set; } = startDate;
    public DateOnly EndDate { get; private set; } = endDate;
}