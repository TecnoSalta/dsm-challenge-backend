using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Domain.Events.Rentals
{
    public class RentalCompletedDomainEvent(Guid id, Guid carId, DateOnly actualReturnDate) : DomainEvent
    {

        public Guid RentalId { get; set; } = id;
        public Guid CarId { get; set; } = carId;
        public DateOnly ActualReturnDate { get; set; } = actualReturnDate;
    }
}
