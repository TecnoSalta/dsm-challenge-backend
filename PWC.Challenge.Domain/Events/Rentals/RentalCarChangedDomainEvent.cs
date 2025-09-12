using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Domain.Events.Rentals;

public class RentalCarChangedDomainEvent(Guid rentalId, Guid oldCarId, Guid newCarId) : DomainEvent
{
    public Guid RentalId { get; } = rentalId;
    public Guid OldCarId { get; } = oldCarId;
    public Guid NewCarId { get; } = newCarId;
    public DateTime ChangedAt { get; } = DateTime.UtcNow;
}