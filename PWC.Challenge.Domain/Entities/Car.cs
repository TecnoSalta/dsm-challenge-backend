using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Enums;

namespace PWC.Challenge.Domain.Entities;

public class Car : AggregateRoot
{
    // Propiedades públicas de solo lectura para EF
    public string Type { get; private set; } = default!;
    public string Model { get; private set; } = default!;
    public CarStatus Status { get; private set; } = CarStatus.Available;
    public IReadOnlyList<Service> Services => _services.AsReadOnly();
    private readonly List<Service> _services = new();

    // Constructor protegido para EF
    protected Car() { }

    // Constructor de dominio
    public Car(Guid id, string type, string model, CarStatus status = CarStatus.Available)
    {
        Id = id;
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Model = model ?? throw new ArgumentNullException(nameof(model));
        Status = status;
    }

    // Ids de ejemplo
    public static readonly Guid Car1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid Car2Id = Guid.Parse("11111111-1111-1111-1111-111111111112");
    public static readonly Guid Car3Id = Guid.Parse("11111111-1111-1111-1111-111111111113");

    // Comportamientos de dominio
    public void ScheduleService(DateOnly date)
    {
        if (_services.Any(s => s.Date == date))
            throw new InvalidOperationException("Service already scheduled on this date.");

        _services.Add(new Service(date));
    }

    public void MarkAsRented()
    {
        if (Status != CarStatus.Available)
            throw new InvalidOperationException("Car is not available to rent.");

        Status = CarStatus.Rented;
    }


    public void MarkAsAvailable()
    {
        if (Status.Equals(CarStatus.Available)) return;
        Status = CarStatus.Available;
    }

    public void MarkAsInMaintenance()
    {
        Status = CarStatus.InMaintenance;
    }

    public bool IsInServicePeriod(DateOnly startDate, DateOnly endDate)
    {
        // Check if the requested period overlaps with any scheduled service
        foreach (var service in Services)
        {
            var serviceStart = service.Date;
            var serviceEnd = service.Date.AddDays(2); // Service lasts 2 days

            if (startDate < serviceEnd && endDate > serviceStart)
            {
                return true;
            }
        }

        return false;
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
}
