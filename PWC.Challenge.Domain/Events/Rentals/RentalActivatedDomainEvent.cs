using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Domain.Events.Rentals;

public class RentalActivatedDomainEvent : DomainEvent
{
    public Guid RentalId { get; }
    public Guid CarId { get; }
    public Guid CustomerId { get; }
    public DateTime ActivatedAt { get; }

    public RentalActivatedDomainEvent(Guid rentalId, Guid carId, Guid customerId)
    {
        RentalId = rentalId;
        CarId = carId;
        CustomerId = customerId;
        ActivatedAt = DateTime.UtcNow;
    }
}