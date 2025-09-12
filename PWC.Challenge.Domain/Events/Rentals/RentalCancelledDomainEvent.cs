using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Domain.Events.Rentals
{
    public sealed class RentalCancelledDomainEvent(Guid rentalId, Guid carId,Guid customerId) : DomainEvent
    {
        public Guid RentalId { get; } = rentalId;
        public Guid CarId { get; } = carId;
        public Guid CustomerId { get; } = customerId;

    }
}