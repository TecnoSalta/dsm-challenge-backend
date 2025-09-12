using MediatR;
using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Application.Common
{
    public class DomainEventNotificationAdapter<T> : INotification where T : DomainEvent
    {
        public T DomainEvent { get; }

        public DomainEventNotificationAdapter(T domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}
