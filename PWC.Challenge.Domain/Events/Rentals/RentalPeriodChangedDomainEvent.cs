using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Domain.Events.Rentals;

public class RentalPeriodChangedDomainEvent(Guid rentalId, int newRentalDays) : DomainEvent
{
    public Guid RentalId { get; } = rentalId;
    public int NewRentalDays { get; } = newRentalDays;
    public DateTime ChangedAt { get; } = DateTime.UtcNow;
}