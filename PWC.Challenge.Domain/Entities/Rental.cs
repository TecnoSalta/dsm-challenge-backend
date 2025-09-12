using MediatR;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Domain.Events.Rentals;
using PWC.Challenge.Domain.Exceptions;
using PWC.Challenge.Domain.ValueObjects;

namespace PWC.Challenge.Domain.Entities;

public class Rental : AggregateRoot
{
    // Properties
    public Guid CustomerId { get; private set; }
    public Guid CarId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public RentalStatus Status { get; private set; } = RentalStatus.Active;
    public RentalPeriod RentalPeriod { get; private set; }
    public decimal TotalCost { get; private set; }
    public decimal DailyRate { get; private set; }
    public DateOnly? ActualReturnDate { get; private set; }

    // Navigation properties
    public Customer Customer { get; private set; } = default!;
    public Car Car { get; private set; } = default!;

    // Private constructor for EF Core
    public Rental() { }

    // Public constructor
    public Rental(Guid id, Customer customer, Car car, DateOnly startDate, DateOnly endDate, decimal dailyRate)
    {
        ValidateRentalPeriod(startDate, endDate);
        ValidateCarAvailability(car, startDate, endDate);

        Id = id;
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        Car = car ?? throw new ArgumentNullException(nameof(car));
        CustomerId = customer.Id;
        CarId = car.Id;
        StartDate = startDate;
        EndDate = endDate;
        DailyRate = dailyRate;
        RentalPeriod = RentalPeriod.Create(startDate, endDate);
        TotalCost = CalculateTotalCost(dailyRate, RentalPeriod.DurationDays); // CAMBIADO: Days → DurationDays
        Status = RentalStatus.Confirmed;

        AddDomainEvent(new RentalCreatedDomainEvent(Id, CarId, CustomerId, startDate, endDate));
    }

    // Business methods
    public async Task CancelAsync(IMediator mediator)
    {
        if (Status != RentalStatus.Confirmed && Status != RentalStatus.Active)
            throw new RentalOperationException("Only confirmed or active rentals can be cancelled.");

        if (StartDate <= DateOnly.FromDateTime(DateTime.UtcNow))
            throw new RentalOperationException("Cannot cancel a rental that has already started.");

        Status = RentalStatus.Cancelled;

        var domainEvent = new RentalCancelledDomainEvent(Id, CarId, CustomerId);
        await mediator.Publish(domainEvent);
    }

    public void UpdateRentalDates(DateOnly newStartDate, DateOnly newEndDate)
    {
        if (Status != RentalStatus.Confirmed)
            throw new RentalOperationException("Only confirmed rentals can be modified.");

        ValidateRentalPeriod(newStartDate, newEndDate);
        ValidateCarAvailability(Car, newStartDate, newEndDate, Id);

        StartDate = newStartDate;
        EndDate = newEndDate;
        RentalPeriod = RentalPeriod.Create(newStartDate, newEndDate);
        TotalCost = CalculateTotalCost(DailyRate, RentalPeriod.DurationDays); // CAMBIADO: Days → DurationDays

        AddDomainEvent(new RentalUpdatedDomainEvent(Id, CarId, newStartDate, newEndDate));
    }

    public void ChangeCar(Car newCar, decimal newDailyRate)
    {
        if (Status != RentalStatus.Confirmed)
            throw new RentalOperationException("Only confirmed rentals can be modified.");

        ValidateCarAvailability(newCar, StartDate, EndDate, Id);

        Car = newCar;
        CarId = newCar.Id;
        DailyRate = newDailyRate;
        TotalCost = CalculateTotalCost(newDailyRate, RentalPeriod.DurationDays); // CAMBIADO: Days → DurationDays

        AddDomainEvent(new RentalCarChangedDomainEvent(Id, CarId, newCar.Id));
    }

    public void MarkAsActive()
    {
        if (Status != RentalStatus.Confirmed)
            throw new RentalOperationException("Only confirmed rentals can be activated.");

        if (StartDate > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new RentalOperationException("Cannot activate a rental before its start date.");

        Status = RentalStatus.Active;

        AddDomainEvent(new RentalActivatedDomainEvent(Id, CarId, CustomerId));
    }

    public void Complete(DateOnly actualReturnDate)
    {
        if (Status != RentalStatus.Active)
            throw new RentalOperationException("Only active rentals can be completed.");

        if (actualReturnDate < StartDate)
            throw new RentalOperationException("Return date cannot be before start date.");

        Status = RentalStatus.Completed;
        ActualReturnDate = actualReturnDate;

        // Calculate any additional charges for late return or discounts for early return
        var actualRentalPeriod = RentalPeriod.Create(StartDate, actualReturnDate);

        // If the actual rental period differs from the planned period, raise event
        if (actualRentalPeriod.DurationDays != RentalPeriod.DurationDays) // CAMBIADO: Days → DurationDays
        {
            AddDomainEvent(new RentalPeriodChangedDomainEvent(Id, actualRentalPeriod.DurationDays)); // CAMBIADO: Days → DurationDays

            // Recalculate cost based on actual rental period
            TotalCost = CalculateTotalCost(DailyRate, actualRentalPeriod.DurationDays); // CAMBIADO: Days → DurationDays
        }

        AddDomainEvent(new RentalCompletedDomainEvent(Id, CarId, CustomerId, actualReturnDate));
    }

    public void ApplyLateFee(decimal lateFeePerDay)
    {
        if (Status != RentalStatus.Completed || !ActualReturnDate.HasValue)
            throw new RentalOperationException("Late fees can only be applied to completed rentals.");

        if (ActualReturnDate > EndDate)
        {
            var lateDays = (ActualReturnDate.Value.ToDateTime(TimeOnly.MinValue) -
                           EndDate.ToDateTime(TimeOnly.MinValue)).Days;

            var lateFee = lateDays * lateFeePerDay;
            TotalCost += lateFee;
            //TODO: Implementar handlers y resto de logica de esta parte.
            //AddDomainEvent(new LateFeeAppliedDomainEvent(Id, lateDays, lateFee));
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

    private void ValidateCarAvailability(Car car, DateOnly startDate, DateOnly endDate, Guid? excludedRentalId = null)
    {
        if (car.IsInServicePeriod(startDate, endDate))
            throw new RentalOperationException("Car is not available due to scheduled service.");

        // Additional availability checks would be implemented here
    }

    // Helper methods
    private decimal CalculateTotalCost(decimal dailyRate, int days)
    {
        // Apply business rules for pricing
        if (days > 7)
        {
            // Weekly discount
            return dailyRate * days * 0.9m;
        }
        else if (days > 14)
        {
            // Bi-weekly discount
            return dailyRate * days * 0.85m;
        }
        else if (days > 28)
        {
            // Monthly discount
            return dailyRate * days * 0.8m;
        }

        return dailyRate * days;
    }

    // Factory method for testing
    public static Rental CreateForTest(Guid id, Customer customer, Car car, DateOnly startDate, DateOnly endDate, decimal dailyRate)
    {
        return new Rental(id, customer, car, startDate, endDate, dailyRate);
    }
}