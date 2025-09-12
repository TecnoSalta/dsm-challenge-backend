using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Domain.Services;

public interface IAvailabilityService
{
    Task<bool> IsCarAvailableAsync(Guid carId, DateOnly startDate, DateOnly endDate, Guid? excludedRentalId = null);
    Task<List<Car>> GetAvailableCarsAsync(DateOnly startDate, DateOnly endDate, string? carType = null, string? model = null);
}