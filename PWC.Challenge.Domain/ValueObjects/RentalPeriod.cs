namespace PWC.Challenge.Domain.ValueObjects;

public record RentalPeriod
{
    public DateOnly StartDate { get; }
    public DateOnly EndDate { get; }
    public int Days { get; }

    private RentalPeriod(DateOnly startDate, DateOnly endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
        Days = (int)(endDate.ToDateTime(TimeOnly.MinValue) - startDate.ToDateTime(TimeOnly.MinValue)).TotalDays;
    }

    public static RentalPeriod Create(DateOnly startDate, DateOnly endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date cannot be after end date.");

        return new RentalPeriod(startDate, endDate);
    }
}