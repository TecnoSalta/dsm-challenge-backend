using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Domain.Interfaces;
using PWC.Challenge.Infrastructure.Data.Common;

namespace PWC.Challenge.Infrastructure.Data.Repositories;

public class RentalRepository : BaseRepository<Rental>, IRentalRepository
{
    public RentalRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> HasOverlappingRentalsAsync(Guid carId, DateOnly startDate, DateOnly endDate, Guid? excludedRentalId = null)
    {
        var query = Context.Set<Rental>()
            .Where(r => r.CarId == carId &&
                       r.Status == RentalStatus.Active &&
                       r.StartDate < endDate &&
                       r.EndDate > startDate);

        if (excludedRentalId.HasValue)
            query = query.Where(r => r.Id != excludedRentalId.Value);

        return await query.AnyAsync();
    }

    public async Task<List<Rental>> GetOverlappingRentalsAsync(Guid carId, DateOnly startDate, DateOnly endDate, Guid? excludedRentalId = null)
    {
        var query = Context.Set<Rental>()
            .Where(r => r.CarId == carId &&
                       r.Status == RentalStatus.Active &&
                       r.StartDate < endDate &&
                       r.EndDate > startDate);

        if (excludedRentalId.HasValue)
            query = query.Where(r => r.Id != excludedRentalId.Value);

        return await query.ToListAsync();
    }
}