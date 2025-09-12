using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Domain.Events.Rentals
{
    public class RentalCompletedDomainEvent(Guid id, Guid carId, Guid customerId,DateOnly actualReturnDate) : DomainEvent
    {

        public Guid RentalId { get; set; } = id;
        public Guid CarId { get; set; } = carId;
        public Guid CustomerId { get; set; } = customerId;

        public DateOnly ActualReturnDate { get; set; } = actualReturnDate;
    }
}
