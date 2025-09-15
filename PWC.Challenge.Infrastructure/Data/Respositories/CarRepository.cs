using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Domain.Interfaces;
using PWC.Challenge.Infrastructure.Data.Common;

namespace PWC.Challenge.Infrastructure.Data.Repositories;

public class CarRepository : BaseRepository<Car>, ICarRepository
{
    public CarRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<Car>> GetAvailableCarsAsync(
        DateOnly startDate,
        DateOnly endDate,
        string? carType = null,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<Car>().AsNoTracking()
            .Include(c => c.Services)
            .Where(c => c.Status == CarStatus.Available)
            // Check services overlap
            .Where(c => !c.Services.Any(s => s.OverlapsWith(startDate, endDate)))
            // Check rentals with buffer day (car cannot be rented the next day after a rental)
            .Where(c => !Context.Set<Rental>()
                .Any(r => r.CarId == c.Id &&
                         r.Status == RentalStatus.Active &&
                         r.RentalPeriod.StartDate < endDate.AddDays(1) &&
                         r.RentalPeriod.EndDate.AddDays(1) > startDate));

        // Apply optional filters
        if (!string.IsNullOrWhiteSpace(carType))
            query = query.Where(c => c.Type.ToLower() == carType.Trim().ToLower());

        if (!string.IsNullOrWhiteSpace(model))
            query = query.Where(c => c.Model.ToLower().Contains(model.Trim().ToLower()));

        return await query.ToListAsync(cancellationToken);
    }

    public Task<List<Car>> GetAvailableCarsAsync(DateOnly startDate, DateOnly endDate, string? carType = null, string? model = null)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsCarAvailableAsync(
        Guid carId,
        DateOnly startDate,
        DateOnly endDate,
        Guid? excludedRentalId = null,
        CancellationToken cancellationToken = default)
    {
        var car = await Context.Set<Car>().AsNoTracking()
            .Include(c => c.Services)
            .FirstOrDefaultAsync(c => c.Id == carId, cancellationToken);

        if (car == null || car.Status != CarStatus.Available)
            return false;

        // Check services overlap
        if (car.Services.Any(s => s.OverlapsWith(startDate, endDate)))
            return false;

        // Check rentals with buffer day (car cannot be rented the next day after a rental)
        var rentalQuery = Context.Set<Rental>()
            .Where(r => r.CarId == carId &&
                       r.Status == RentalStatus.Active &&
                       r.RentalPeriod.StartDate < endDate.AddDays(1) &&
                       r.RentalPeriod.EndDate.AddDays(1) > startDate);

        if (excludedRentalId.HasValue)
            rentalQuery = rentalQuery.Where(r => r.Id != excludedRentalId.Value);

        return !await rentalQuery.AnyAsync(cancellationToken);
    }

   
}