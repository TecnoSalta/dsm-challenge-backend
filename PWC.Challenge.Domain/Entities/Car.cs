using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Domain.Cars;

public class Car:Entity
{
    public Guid Id { get; private set; }
    public string Type { get; private set; }
    public string Model { get; private set; }
    public List<Service> Services { get; private set; }
    //Todo hacer enumerado
    public string Status { get; set; } ="available";

    public static readonly Guid Car1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public static readonly Guid Car2Id = Guid.Parse("11111111-1111-1111-1111-111111111112");

    public static readonly Guid Car3Id = Guid.Parse("11111111-1111-1111-1111-111111111113");

    public Car(Guid id, string type, string model)
    {
        Id = id;
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Model = model ?? throw new ArgumentNullException(nameof(model));
        Services = new List<Service>();
    }

    public void ScheduleService(DateOnly date)
    {
        Services.Add(new Service(date));
    }
}