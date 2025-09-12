using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Domain.Events.Rentals
{
    public class RentalCreatedDomainEvent(Guid id, Guid carId, Guid customerId, DateOnly startDate, DateOnly endDate) : DomainEvent
    {
        public Guid RentalId { get; set; } = id;
        public Guid CarId { get; set; } = carId;
        public Guid CustomerId{ get; set; } = customerId;
        public DateOnly StartDate { get; set; } = startDate;

        public DateOnly EndDate { get; set; } = endDate;
    }
}
