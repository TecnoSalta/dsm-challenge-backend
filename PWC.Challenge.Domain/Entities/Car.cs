namespace PWC.Challenge.Domain.Cars;

public class Car
{
    public Guid Id { get; private set; }
    public string Type { get; private set; }
    public string Model { get; private set; }
    public List<Service> Services { get; private set; }

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