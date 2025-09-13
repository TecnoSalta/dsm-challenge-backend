using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Domain.Entities;

public class Service : ValueObject
{
    public DateOnly Date { get; private set; }
    public int DurationDays { get; private set; } = 2;

    private Service() { }

    public Service(DateOnly date, int durationDays = 2)
    {
        Date = date;
        DurationDays = durationDays;
    }

    public bool OverlapsWith(DateOnly startDate, DateOnly endDate)
    {
        var serviceEnd = Date.AddDays(DurationDays);
        return startDate < serviceEnd && endDate > Date;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Date;
        yield return DurationDays;
    }
}