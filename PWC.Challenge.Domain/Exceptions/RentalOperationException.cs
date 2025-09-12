namespace PWC.Challenge.Domain.Exceptions;

public class RentalOperationException : Exception
{
    public RentalOperationException(string message) : base(message)
    {
    }

    public RentalOperationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}