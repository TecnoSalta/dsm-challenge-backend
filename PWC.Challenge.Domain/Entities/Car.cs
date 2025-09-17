using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Domain.Exceptions;
using PWC.Challenge.Domain.ValueObjects;
using PWC.Challenge.Domain.Events;

namespace PWC.Challenge.Domain.Entities;

public class Car : AggregateRoot
{
    // Propiedades públicas de solo lectura para EF
    public string Type { get; private set; } = default!;
    public string Model { get; private set; } = default!;
    public decimal DailyRate { get; private set; }
    public CarStatus Status { get; private set; } = CarStatus.Available;
    public string LicensePlate { get; private set; } = default!;
    public IReadOnlyList<Service> Services => _services.AsReadOnly();
    public IReadOnlyList<Rental> Rentals => _rentals.AsReadOnly();

    private readonly List<Service> _services = new();
    private readonly List<Rental> _rentals = new();

    // Constructor protegido para EF
    protected Car() { }

    // Constructor de dominio
    public Car(Guid id, string type, string model, decimal dailyRate, string licencePlate, CarStatus status = CarStatus.Available)
    {
        Id = id;
        Type = string.IsNullOrWhiteSpace(type) ? throw new InvalidCarArgumentException(nameof(type)) : type;
        Model = string.IsNullOrWhiteSpace(model) ? throw new InvalidCarArgumentException(nameof(model)) : model;
        DailyRate = dailyRate;
        LicensePlate = licencePlate ?? throw new InvalidCarArgumentException(nameof(licencePlate));
        Status = status;
    }

    // Ids de ejemplo
    public static readonly Guid Car1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid Car2Id = Guid.Parse("11111111-1111-1111-1111-111111111112");
    public static readonly Guid Car3Id = Guid.Parse("11111111-1111-1111-1111-111111111113");

    // ========== MANEJO DE ESTADOS - MÁQUINA DE ESTADOS DDD ==========

    public void Reserve(DateOnly startDate, DateOnly endDate, Guid rentalId)
    {
        if (Status != CarStatus.Available)
            throw new CarNotAvailableException(Id);

        if (IsInServicePeriod(startDate, endDate))
            throw new CarInServicePeriodException(Id, startDate, endDate);

        if (HasOverlappingRentals(startDate, endDate))
            throw new OverlappingRentalException(Id, startDate, endDate);

        Status = CarStatus.Reserved;
        AddDomainEvent(new CarReservedDomainEvent(Id, rentalId, startDate, endDate));
    }

    public void CancelReservation(Guid rentalId)
    {
        if (Status != CarStatus.Reserved)
            throw new InvalidStateTransitionException(Id, Status, CarStatus.Available, "Can only cancel from Reserved status");

        Status = CarStatus.Available;
        AddDomainEvent(new CarReservationCancelledDomainEvent(Id, rentalId));
    }

    public void ConfirmPickup(Guid rentalId)
    {
        if (Status != CarStatus.Reserved)
            throw new InvalidStateTransitionException(Id, Status, CarStatus.Rented, "Can only pickup from Reserved status");

        Status = CarStatus.Rented;
        AddDomainEvent(new CarPickedUpDomainEvent(Id, rentalId, DateTime.UtcNow));
    }

    public void ProcessReturn(Guid rentalId)
    {
        if (Status != CarStatus.Rented)
            throw new InvalidStateTransitionException(Id, Status, CarStatus.PendingCleaning, "Can only return from Rented status");

        Status = CarStatus.PendingCleaning;
        AddDomainEvent(new CarReturnedDomainEvent(Id, rentalId, DateTime.UtcNow));
    }

    public void CompleteCleaning()
    {
        if (Status != CarStatus.PendingCleaning)
            throw new InvalidStateTransitionException(Id, Status, CarStatus.Available, "Can only complete cleaning from PendingCleaning status");

        Status = CarStatus.Available;
        AddDomainEvent(new CarCleaningCompletedDomainEvent(Id, DateTime.UtcNow));
    }

    public void SendToMaintenance(string reason = "Scheduled maintenance")
    {
        if (Status == CarStatus.Rented)
            throw new CarRentedCannotBeInMaintenanceException();

        if (Status == CarStatus.Reserved)
            throw new InvalidStateTransitionException(Id, Status, CarStatus.InMaintenance, "Cannot send reserved car to maintenance");

        var previousStatus = Status;
        Status = CarStatus.InMaintenance;
        AddDomainEvent(new CarSentToMaintenanceDomainEvent(Id, previousStatus, reason, DateTime.UtcNow));
    }

    public void CompleteMaintenance()
    {
        if (Status != CarStatus.InMaintenance)
            throw new InvalidStateTransitionException(Id, Status, CarStatus.Available, "Can only complete maintenance from InMaintenance status");

        Status = CarStatus.Available;
        AddDomainEvent(new CarMaintenanceCompletedDomainEvent(Id, DateTime.UtcNow));
    }

    public void ForceAvailable(string reason)
    {
        var previousStatus = Status;
        Status = CarStatus.Available;
        AddDomainEvent(new CarStatusForcedDomainEvent(Id, previousStatus, CarStatus.Available, reason, DateTime.UtcNow));
    }

    public void AddRental(Rental rental)
    {
        if (rental == null)
            throw new ArgumentNullException(nameof(rental));

        // Ensure the car is available for the rental period, considering the buffer
        if (!IsAvailableForPeriod(rental.RentalPeriod.StartDate, rental.RentalPeriod.EndDate))
        {
            throw new OverlappingRentalException(Id, rental.RentalPeriod.StartDate, rental.RentalPeriod.EndDate);
        }

        _rentals.Add(rental);
        AddDomainEvent(new CarRentalAddedDomainEvent(Id, rental.Id, rental.RentalPeriod.StartDate, rental.RentalPeriod.EndDate));
    }

    // ========== MÉTODOS PRIVADOS ==========

    private bool HasOverlappingRentals(DateOnly startDate, DateOnly endDate, Guid? excludedRentalId = null)
    {
        return _rentals.Any(r =>
            r.Id != excludedRentalId &&
            (r.Status == RentalStatus.Active || r.Status == RentalStatus.Reserved) &&
            r.RentalPeriod.OverlapsWith(new RentalPeriod(startDate, endDate.AddDays(1)))); // 1-day buffer
    }

    public bool IsInServicePeriod(DateOnly startDate, DateOnly endDate)
    {
        return _services.Any(service => service.OverlapsWith(startDate, endDate));
    }

    public bool IsAvailableForPeriod(DateOnly startDate, DateOnly endDate)
    {
        // Check for overlapping services
        if (IsInServicePeriod(startDate, endDate))
            return false;

        // Check for overlapping rentals, considering the 1-day buffer
        if (HasOverlappingRentals(startDate, endDate))
            return false;

        // Also consider the car's current status if it's not available
        if (Status != CarStatus.Available)
            return false;

        return true;
    }

    // ========== MÉTODOS DE CONSULTA DE ESTADO ==========

    public bool CanBeReserved() => Status == CarStatus.Available;
    public bool CanBePickedUp() => Status == CarStatus.Reserved;
    public bool CanBeReturned() => Status == CarStatus.Rented;
    public bool CanGoToMaintenance() => Status != CarStatus.Rented && Status != CarStatus.Reserved;

    public IEnumerable<CarStatus> GetValidTransitions()
    {
        return Status switch
        {
            CarStatus.Available => new[] { CarStatus.Reserved, CarStatus.InMaintenance },
            CarStatus.Reserved => new[] { CarStatus.Available, CarStatus.Rented },
            CarStatus.Rented => new[] { CarStatus.PendingCleaning },
            CarStatus.PendingCleaning => new[] { CarStatus.Available, CarStatus.InMaintenance },
            CarStatus.InMaintenance => new[] { CarStatus.Available },
            _ => Array.Empty<CarStatus>()
        };
    }

    // Método de ayuda para pruebas
    public void AddService(DateOnly date, int durationDays)
    {
        _services.Add(new Service(date, durationDays));
    }
}
