using MediatR;

namespace PWC.Challenge.Application.Features.Rentals.Events
{
    internal class RentalCancelledDomainEvent : INotification
    {
        public Guid CarId { get; set; }
        // Otros miembros según sea necesario
    }
}
