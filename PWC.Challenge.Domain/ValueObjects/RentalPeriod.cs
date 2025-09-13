using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Domain.ValueObjects;

public class RentalPeriod : ValueObject
{
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public int DurationDays => EndDate.DayNumber - StartDate.DayNumber;

    private RentalPeriod() { }

    public RentalPeriod(DateOnly startDate, DateOnly endDate)
    {
        if (startDate >= endDate)
            throw new ArgumentException("End date must be after start date");

        StartDate = startDate;
        EndDate = endDate;
    }

    // Método Factory para compatibilidad con código existente
    public static RentalPeriod Create(DateOnly startDate, DateOnly endDate)
    {
        return new RentalPeriod(startDate, endDate);
    }

    public bool OverlapsWith(RentalPeriod other)
    {
        return StartDate < other.EndDate && EndDate > other.StartDate;
    }

    public bool OverlapsWith(DateOnly start, DateOnly end)
    {
        return StartDate < end && EndDate > start;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
    }
}