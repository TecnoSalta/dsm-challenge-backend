using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Domain.Entities;

public class Service : Entity
{
    public DateOnly Date { get; private set; }
    public int DurationDays { get; private set; } = 2;
    public Guid CarId { get; private set; }
    public Car Car { get; private set; } = null!;

    private Service() { }

    public Service(DateOnly date, Guid carId, int durationDays = 2)
    {
        Date = date;
        CarId = carId;
        DurationDays = durationDays;
    }

    public bool OverlapsWith(DateOnly startDate, DateOnly endDate)
    {
        var serviceEnd = Date.AddDays(DurationDays);
        return startDate < serviceEnd && endDate > Date;
    }
}