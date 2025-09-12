
namespace PWC.Challenge.Domain.Common;

public interface IHasDomainEvents
{
    List<DomainEvent> DomainEvents { get; }
    void AddDomainEvent(DomainEvent eventItem);
    void ClearDomainEvents();
}

public abstract class BaseEntity : IHasDomainEvents
{
    private List<DomainEvent>? _domainEvents;

    public List<DomainEvent> DomainEvents => _domainEvents ??= new List<DomainEvent>();

    public void AddDomainEvent(DomainEvent eventItem)
    {
        DomainEvents.Add(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }
}
