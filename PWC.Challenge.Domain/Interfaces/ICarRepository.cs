using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Domain.Interfaces;

public interface ICarRepository : IBaseRepository<Car>
{
    Task<List<Car>> GetAvailableCarsAsync(DateOnly startDate, DateOnly endDate, string? carType = null, string? model = null);
    Task<bool> IsCarAvailableAsync(Guid carId, DateOnly startDate, DateOnly endDate, Guid? excludedRentalId = null);
}