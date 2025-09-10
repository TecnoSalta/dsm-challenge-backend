using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Domain.Entities;

public class Service:Entity
{
    public DateOnly Date { get; private set; }

    public Service(DateOnly date)
    {
        Date = date;
    }
}