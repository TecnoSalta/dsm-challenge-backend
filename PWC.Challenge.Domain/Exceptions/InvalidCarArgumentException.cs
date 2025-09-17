namespace PWC.Challenge.Domain.Exceptions;

public class InvalidCarArgumentException : DomainException
{
    public InvalidCarArgumentException(string parameterName) 
        : base($"Argument '{parameterName}' is invalid for car creation.")
    {
    }
}
