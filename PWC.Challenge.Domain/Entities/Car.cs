using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Domain.Entities;

public class Car : Entity
{
    // Propiedades públicas que EF puede rellenar
    public Guid Id { get; private init; } = default!;
    public string Type { get; private set; } = default!;
    public string Model { get; private set; } = default!;
    public string Status { get; private set; } = "available";
    public List<Service> Services { get; private set; } = new();

    // ctor sin parámetros para EF
    private Car() { }

    // ctor de dominio para tu código
    public Car(Guid id, string type, string model, string status = "available")
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