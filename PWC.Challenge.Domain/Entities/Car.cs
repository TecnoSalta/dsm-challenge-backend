using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Domain.Exceptions;

namespace PWC.Challenge.Domain.Entities;

public class Car : AggregateRoot
{
    // Propiedades públicas de solo lectura para EF
    public string Type { get; private set; } = default!;
    public string Model { get; private set; } = default!;
    public decimal DailyRate { get; private set; }
    public CarStatus Status { get; private set; } = CarStatus.Available;
    public IReadOnlyList<Service> Services => _services.AsReadOnly();
    public IReadOnlyList<Rental> Rentals => _rentals.AsReadOnly();

    private readonly List<Service> _services = new();
    private readonly List<Rental> _rentals = new();

    // Constructor protegido para EF
    protected Car() { }

    // Constructor de dominio
    public Car(Guid id, string type, string model, decimal dailyRate, CarStatus status = CarStatus.Available)
    {
        Id = id;
        Type = string.IsNullOrWhiteSpace(type) ? throw new InvalidCarArgumentException(nameof(type)) : type;
        Model = string.IsNullOrWhiteSpace(model) ? throw new InvalidCarArgumentException(nameof(model)) : model;
        DailyRate = dailyRate;
        Status = status;
    }

    // Ids de ejemplo
    public static readonly Guid Car1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid Car2Id = Guid.Parse("11111111-1111-1111-1111-111111111112");
    public static readonly Guid Car3Id = Guid.Parse("11111111-1111-1111-1111-111111111113");

    // Comportamientos de dominio
    public void ScheduleService(DateOnly date, int durationDays = 2)
    {
        if (_services.Any(s => s.OverlapsWith(date, date.AddDays(durationDays))))
            throw new OverlappingServiceException();

        _services.Add(new Service(date, durationDays));
    }

    public void AddRental(Rental rental)
    {
        if (rental == null) throw new ArgumentNullException(nameof(rental));
        if (rental.CarId != Id) throw new RentalCarMismatchException();

        // Un agregado raíz debe proteger sus invariantes.
        // Un coche no puede tener múltiples alquileres activos que se solapen.
        if (rental.Status == RentalStatus.Active && _rentals.Any(r => r.Id != rental.Id && r.Status == RentalStatus.Active && r.RentalPeriod.OverlapsWith(rental.RentalPeriod)))
        {
            throw new OverlappingRentalException();
        }

        _rentals.Add(rental);
    }

    public void MarkAsRented()
    {
        if (Status != CarStatus.Available)
            throw new CarNotAvailableException(Id);

        Status = CarStatus.Rented;
    }

    public void MarkAsAvailable()
    {
        if (Status.Equals(CarStatus.Available)) return;
        Status = CarStatus.Available;
    }

    public void MarkAsInMaintenance()
    {
        if (Status == CarStatus.Rented)
            throw new CarRentedCannotBeInMaintenanceException();

        Status = CarStatus.InMaintenance;
    }

    public bool IsInServicePeriod(DateOnly startDate, DateOnly endDate)
    {
        return _services.Any(service => service.OverlapsWith(startDate, endDate));
    }

    public bool IsAvailableForRental(DateOnly startDate, DateOnly endDate, Guid? excludedRentalId = null)
    {
        // Check service periods
        if (IsInServicePeriod(startDate, endDate))
            return false;

        // Check existing rentals (this would typically be handled by a domain service)
        // For simplicity, we assume a method exists to check for overlapping rentals
        return true;
    }

    // Método de ayuda para pruebas
    public void AddService(DateOnly date, int durationDays)
    {
        _services.Add(new Service(date, durationDays));
    }
}