using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Domain.Interfaces;

public interface IRentalRepository : IBaseRepository<Rental>
{
    Task<bool> HasOverlappingRentalsAsync(Guid carId, DateOnly startDate, DateOnly endDate, Guid? excludedRentalId = null);
    Task<List<Rental>> GetOverlappingRentalsAsync(Guid carId, DateOnly startDate, DateOnly endDate, Guid? excludedRentalId = null);
    Task<List<Rental>> GetRentalsByEndDateAsync(Guid carId, DateOnly endDate, Guid? excludedRentalId = null);
    Task<IEnumerable<Rental>> GetAllWithCustomersAsync(bool asNoTracking = false, CancellationToken cancellationToken = default);
}