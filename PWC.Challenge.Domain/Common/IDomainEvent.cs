

namespace PWC.Challenge.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
