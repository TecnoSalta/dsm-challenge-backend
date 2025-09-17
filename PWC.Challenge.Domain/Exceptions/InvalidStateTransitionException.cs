using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Domain.Exceptions;

namespace PWC.Challenge.Domain.Exceptions;

public class InvalidStateTransitionException : DomainException
{
    public InvalidStateTransitionException(Guid entityId, Enum currentState, Enum targetState, string message)
        : base($"Invalid state transition for entity {entityId}. Cannot change from {currentState} to {targetState}. {message}")
    {
    }
}
