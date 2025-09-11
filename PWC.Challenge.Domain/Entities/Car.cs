using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Enums;

namespace PWC.Challenge.Domain.Entities;

public class Car : Entity
{
    // Propiedades públicas que EF puede rellenar
    public Guid Id { get;  init; } = default!;
    public string Type { get;  set; } = default!;
    public string Model { get;  set; } = default!;
    public CarStatus Status { get;  set; } = CarStatus.Available;
    public List<Service> Services { get;  set; } = new();

    // ctor sin parámetros para EF
    private Car() { }

    // ctor de dominio para tu código
    public Car(Guid id, string type, string model, CarStatus status = CarStatus.Available)
    {
        Id = id;
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Model = model ?? throw new ArgumentNullException(nameof(model));
        Status = status;
    }

    public static readonly Guid Car1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid Car2Id = Guid.Parse("11111111-1111-1111-1111-111111111112");
    public static readonly Guid Car3Id = Guid.Parse("11111111-1111-1111-1111-111111111113");

    public void ScheduleService(DateOnly date)
        => Services.Add(new Service(date));
}