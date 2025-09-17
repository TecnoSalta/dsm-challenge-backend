namespace PWC.Challenge.Domain.Exceptions;

public class OverlappingServiceException : DomainException
{
    public OverlappingServiceException() 
        : base("Service overlaps with existing service.")
    {
    }
}
