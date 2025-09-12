using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Domain.Rentals
{
    public sealed class RentalCancelledDomainEvent(Guid rentalId, Guid carId) : DomainEvent
    {
        public Guid RentalId { get; } = rentalId;
        public Guid CarId { get; } = carId;
    }
}