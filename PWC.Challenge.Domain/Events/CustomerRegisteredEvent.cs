using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Domain.Events;

// Domain/Events/CustomerRegisteredEvent.cs

public class CustomerRegisteredEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public DateTime OccurredOn { get; }

    public CustomerRegisteredEvent(Guid customerId)
    {
        CustomerId = customerId;
        OccurredOn = DateTime.UtcNow;
    }
}