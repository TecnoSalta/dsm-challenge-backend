using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Domain.Events.Rentals;
using PWC.Challenge.Domain.Exceptions;
using PWC.Challenge.Domain.ValueObjects;
using PWC.Challenge.Domain.Services; // Keep this if IsInServicePeriod is used

namespace PWC.Challenge.Domain.Entities;

public class Rental : AggregateRoot
{
    // Properties
    public Guid CustomerId { get; private set; }
    public Guid CarId { get; private set; }
    public RentalStatus Status { get; private set; } = RentalStatus.Active;
    public RentalPeriod RentalPeriod { get; private set; }
    public decimal TotalCost { get; private set; }
    public decimal DailyRate { get; private set; }
    public DateOnly? ActualReturnDate { get; private set; }

    // Navigation properties
    public Customer Customer { get; private set; } = default!;
    public Car Car { get; private set; } = default!;

    // Private constructor for EF Core
    private Rental() { }

    // Protected constructor for internal use by factory method and EF Core
    protected Rental(Guid id, Customer customer, Car car, DateOnly startDate, DateOnly endDate, decimal dailyRate)
    {
        // Initial validations that don't require external services
        ValidateRentalPeriod(startDate, endDate);
        // ValidateCarAvailability is moved out of here, as it requires external service (repository)

        Id = id;
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        Car = car ?? throw new ArgumentNullException(nameof(car));
        CustomerId = customer.Id;
        CarId = car.Id;
        DailyRate = dailyRate;
        RentalPeriod = RentalPeriod.Create(startDate, endDate);
        TotalCost = CalculateTotalCost(dailyRate, RentalPeriod.DurationDays);
        Status = RentalStatus.Confirmed;

        AddDomainEvent(new RentalCreatedDomainEvent(Id, CarId, CustomerId, startDate, endDate));
    }

    // Factory method
    public static Rental Create(Guid id, Customer customer, Car car, DateOnly startDate, DateOnly endDate, decimal dailyRate)
    {
        // Perform validations that are part of the entity's invariant
        var rental = new Rental(id, customer, car, startDate, endDate, dailyRate);
        // Additional domain-specific validations can go here if needed before returning
        return rental;
    }

    // Business methods
    public void Cancel()
    {
        if (Status != RentalStatus.Confirmed && Status != RentalStatus.Active)
            throw new RentalOperationException("Only confirmed or active rentals can be cancelled.");

        if (RentalPeriod.StartDate <= DateOnly.FromDateTime(DateTime.UtcNow))
            throw new RentalOperationException("Cannot cancel a rental that has already started.");

        Status = RentalStatus.Cancelled;

        AddDomainEvent(new RentalCancelledDomainEvent(Id, CarId, CustomerId));
    }

    public void UpdateRentalDates(DateOnly newStartDate, DateOnly newEndDate)
    {
        if (Status != RentalStatus.Confirmed && Status != RentalStatus.Active)
            throw new RentalOperationException("Only confirmed or active rentals can be modified.");

        ValidateRentalPeriod(newStartDate, newEndDate);
        // ValidateCarAvailability is moved out of here

        RentalPeriod = RentalPeriod.Create(newStartDate, newEndDate);
        TotalCost = CalculateTotalCost(DailyRate, RentalPeriod.DurationDays);

        AddDomainEvent(new RentalUpdatedDomainEvent(Id, CarId, newStartDate, newEndDate));
    }

    public void ChangeCar(Car newCar, decimal newDailyRate)
    {
        if (Status != RentalStatus.Confirmed && Status != RentalStatus.Active)
            throw new RentalOperationException("Only confirmed or active rentals can be modified.");

        // ValidateCarAvailability is moved out of here

        Car = newCar;
        CarId = newCar.Id;
        DailyRate = newDailyRate;
        TotalCost = CalculateTotalCost(newDailyRate, RentalPeriod.DurationDays);

        AddDomainEvent(new RentalCarChangedDomainEvent(Id, CarId, newCar.Id));
    }

    public void MarkAsActive()
    {
        if (Status != RentalStatus.Confirmed)
            throw new RentalOperationException("Only confirmed rentals can be activated.");

        if (RentalPeriod.StartDate > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new RentalOperationException("Cannot activate a rental before its start date.");

        Status = RentalStatus.Active;

        AddDomainEvent(new RentalActivatedDomainEvent(Id, CarId, CustomerId));
    }

    public void Complete(DateOnly actualReturnDate)
    {
        if (Status != RentalStatus.Active)
            throw new RentalOperationException("Only active rentals can be completed.");

        if (actualReturnDate < RentalPeriod.StartDate)
            throw new RentalOperationException("Return date cannot be before start date.");

        Status = RentalStatus.Completed;
        ActualReturnDate = actualReturnDate;

        var actualRentalPeriod = RentalPeriod.Create(RentalPeriod.StartDate, actualReturnDate);

        if (actualRentalPeriod.DurationDays != RentalPeriod.DurationDays)
        {
            AddDomainEvent(new RentalPeriodChangedDomainEvent(Id, actualRentalPeriod.DurationDays));
            TotalCost = CalculateTotalCost(DailyRate, actualRentalPeriod.DurationDays);
        }

        AddDomainEvent(new RentalCompletedDomainEvent(Id, CarId, CustomerId, actualReturnDate));
    }

    public void ApplyLateFee(decimal lateFeePerDay)
    {
        if (Status != RentalStatus.Completed || !ActualReturnDate.HasValue)
            throw new RentalOperationException("Late fees can only be applied to completed rentals.");

        if (ActualReturnDate > RentalPeriod.EndDate)
        {
            var lateDays = (ActualReturnDate.Value.DayNumber -
                           RentalPeriod.EndDate.DayNumber);

            var lateFee = lateDays * lateFeePerDay;
            TotalCost += lateFee;
        }
    }

    // Validation methods
    private void ValidateRentalPeriod(DateOnly startDate, DateOnly endDate)
    {
        if (startDate < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new RentalOperationException("Start date cannot be in the past.");

        if (startDate > endDate)
            throw new RentalOperationException("Start date cannot be after end date.");

        var maxRentalPeriod = TimeSpan.FromDays(30);
        if ((endDate.ToDateTime(TimeOnly.MinValue) - startDate.ToDateTime(TimeOnly.MinValue)).TotalDays > maxRentalPeriod.TotalDays)
            throw new RentalOperationException($"Rental period cannot exceed {maxRentalPeriod.TotalDays} days.");
    }

    // Helper methods
    private decimal CalculateTotalCost(decimal dailyRate, int days)
    {
        if (days > 7)
        {
            return dailyRate * days * 0.9m;
        }
        else if (days > 14)
        {
            return dailyRate * days * 0.85m;
        }
        else if (days > 28)
        {
            return dailyRate * days * 0.8m;
        }

        return dailyRate * days;
    }

    // Factory method for testing - this can be removed or kept if still needed for specific testing scenarios
    public static Rental CreateForTest(Guid id, Customer customer, Car car, DateOnly startDate, DateOnly endDate, decimal dailyRate)
    {
        var rental = new Rental { Id = id, Customer = customer, Car = car, DailyRate = dailyRate, Status = RentalStatus.Confirmed };
        rental.RentalPeriod = RentalPeriod.Create(startDate, endDate);
        rental.TotalCost = rental.CalculateTotalCost(dailyRate, rental.RentalPeriod.DurationDays);
        return rental;
    }
}