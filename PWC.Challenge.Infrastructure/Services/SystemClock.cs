using PWC.Challenge.Application.Interfaces;

namespace PWC.Challenge.Infrastructure.Services;

public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
