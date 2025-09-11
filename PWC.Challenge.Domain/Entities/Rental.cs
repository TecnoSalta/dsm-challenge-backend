using Domain.Rentals;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Enums;

namespace PWC.Challenge.Domain.Entities;

public class Rental : AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public Guid CarId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public RentalStatus Status { get; private set; } = RentalStatus.Active;

    public Customer Customer { get; private set; } = default!;
    public Car Car { get; private set; } = default!;

    // Constructor vacío protegido para EF
    protected Rental() { }

    public Rental(Guid id, Customer customer, Car car, DateOnly startDate, DateOnly endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("StartDate cannot be after EndDate.");

        Id = id;
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        Car = car ?? throw new ArgumentNullException(nameof(car));
        CustomerId = customer.Id;
        CarId = car.Id;
        StartDate = startDate;
        EndDate = endDate;
    }

    // Cancelar reserva
    public void Cancel()
    {
        if (Status != RentalStatus.Active)
            throw new InvalidOperationException("Only active rentals can be cancelled.");

        Status = RentalStatus.Cancelled;

        AddDomainEvent(new RentalCancelledDomainEvent(this.Id, this.CarId));
    }

    // Actualizar fechas y/o coche
    // TODO: mejorar las reglas de negocio y agregar tests
    public void UpdateRental(DateOnly newStart, DateOnly newEnd, Car? newCar = null)
    {
        if (newStart > newEnd)
            throw new ArgumentException("StartDate cannot be after EndDate.");

        StartDate = newStart;
        EndDate = newEnd;

        if (newCar != null)
        {
            Car = newCar;
            CarId = newCar.Id;
        }
    }
}
