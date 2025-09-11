using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Enums;

namespace PWC.Challenge.Domain.Entities;

public class Rental: Entity
{
    public Guid Id { get;  set; }
    public Guid CustomerId { get;  set; }
    public Guid CarId { get;  set; }
    public DateOnly StartDate { get;  set; }
    public DateOnly EndDate { get;  set; }

    public RentalStatus Status { get; set; } = RentalStatus.Active;

    public Customer Customer { get;  set; } = default!;
    public Car Car { get;  set; } = default!;

    // ctor vacío para EF
    private Rental() { }

    // ctor escalar que EF también puede usar
    private Rental(Guid id, Guid customerId, Guid carId, DateOnly startDate, DateOnly endDate)
    {
        Id = id;
        CustomerId = customerId;
        CarId = carId;
        StartDate = startDate;
        EndDate = endDate;
    }

    // ctor rico para tu dominio
    public Rental(Guid id, Customer customer, Car car, DateOnly startDate, DateOnly endDate)
    {
        Id = id;
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        Car = car ?? throw new ArgumentNullException(nameof(car));
        CustomerId = customer.Id;
        CarId = car.Id;
        StartDate = startDate;
        EndDate = endDate;
    }
}
