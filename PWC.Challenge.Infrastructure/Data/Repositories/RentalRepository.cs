using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Domain.Interfaces;
using PWC.Challenge.Infrastructure.Data.Common;
using PWC.Challenge.Infrastructure.Data.Extensions;

namespace PWC.Challenge.Infrastructure.Data.Repositories;

public class RentalRepository : BaseRepository<Rental>, IRentalRepository
{
    public RentalRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Rental>> GetAllWithCustomersAsync(bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var query = Context.Set<Rental>()
            .Include(r => r.Customer)
            .AsNoTrackingIf(asNoTracking);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<bool> HasOverlappingRentalsAsync(Guid carId, DateOnly startDate, DateOnly endDate, Guid? excludedRentalId = null)
    {
        var query = Context.Set<Rental>()
            .Where(r => r.CarId == carId &&
                       r.Status != RentalStatus.Cancelled &&
                       r.RentalPeriod.StartDate < endDate &&
                       r.RentalPeriod.EndDate > startDate);

        if (excludedRentalId.HasValue)
            query = query.Where(r => r.Id != excludedRentalId.Value);

        return await query.AnyAsync();
    }

    public async Task<List<Rental>> GetOverlappingRentalsAsync(Guid carId, DateOnly startDate, DateOnly endDate, Guid? excludedRentalId = null)
    {
        var query = Context.Set<Rental>()
            .Where(r => r.CarId == carId &&
                       r.Status != RentalStatus.Cancelled &&
                       r.RentalPeriod.StartDate < endDate &&
                       r.RentalPeriod.EndDate > startDate);

        if (excludedRentalId.HasValue)
            query = query.Where(r => r.Id != excludedRentalId.Value);

        return await query.ToListAsync();
    }
    public async Task<List<Rental>> GetRentalsByEndDateAsync(Guid carId, DateOnly endDate, Guid? excludedRentalId = null)
    {
        var query = Context.Set<Rental>()
            .Where(r => r.CarId == carId &&
                        r.RentalPeriod.EndDate == endDate &&
                        r.Status != RentalStatus.Cancelled);

        if (excludedRentalId.HasValue)
            query = query.Where(r => r.Id != excludedRentalId.Value);

        return await query.ToListAsync();
    }
}