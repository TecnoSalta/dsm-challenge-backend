using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Domain.Events.Rentals
{
    public class RentalUpdatedDomainEvent(Guid id, Guid carId,  DateOnly startDate, DateOnly endDate) : DomainEvent
    {
        public Guid RentalId { get; set; } = id;
        public Guid CarId { get; set; } = carId;
        public DateOnly StartDate { get; set; } = startDate;
        public DateOnly EndDate { get; set; } = endDate;
    }
}
