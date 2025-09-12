using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Domain.Interfaces;
/// <summary>
/// Car Service Repository interface
/// </summary>
public interface IServiceRepository : IBaseRepository<Service>
{
    Task<bool> HasScheduledServicesAsync(Guid carId, DateOnly startDate, DateOnly endDate);
    Task<List<Service>> GetScheduledServicesAsync(Guid carId, DateOnly startDate, DateOnly endDate);
}