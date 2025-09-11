using PWC.Challenge.Domain.Common;

namespace Domain.Rentals
{
    public record RentalCancelledDomainEvent(Guid RentalId, Guid CarId) : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}