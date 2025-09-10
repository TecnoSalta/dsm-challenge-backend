namespace PWC.Challenge.Domain.Cars;

public class Service
{
    public DateOnly Date { get; private set; }

    public Service(DateOnly date)
    {
        Date = date;
    }
}