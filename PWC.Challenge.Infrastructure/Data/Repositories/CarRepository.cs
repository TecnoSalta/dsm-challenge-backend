using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Domain.Interfaces;
using PWC.Challenge.Infrastructure.Data.Common;
using PWC.Challenge.Infrastructure.Data.Extensions;

namespace PWC.Challenge.Infrastructure.Data.Repositories;

public class CarRepository : BaseRepository<Car>, ICarRepository
{
    public CarRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Car>> GetAllWithServicesAsync(bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var query = Context.Set<Car>()
            .Include(c => c.Services)
            .AsNoTrackingIf(asNoTracking);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<Car>> GetAvailableCarsAsync(
        DateOnly startDate,
        DateOnly endDate,
        string? carType = null,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<Car>()
            .Include(c => c.Rentals)
            .Include(c => c.Services)
            .Where(c => c.Status == CarStatus.Available)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(carType))
        {
            query = query.Where(c => c.Type == carType);
        }

        if (!string.IsNullOrEmpty(model))
        {
            query = query.Where(c => c.Model == model);
        }

        var cars = await query.ToListAsync(cancellationToken);

        // Filter in memory based on overlapping rentals and services
        return cars.Where(car =>
            !car.Rentals.Any(r =>
                r.Status == RentalStatus.Active &&
                r.RentalPeriod.StartDate < endDate &&
                r.RentalPeriod.EndDate > startDate)
            &&
            !car.Services.Any(s =>
                s.OverlapsWith(startDate, endDate)) // Assuming Service.OverlapsWith method exists
        ).ToList();
    }

    public async Task<bool> IsCarAvailableAsync(
        Guid carId,
        DateOnly startDate,
        DateOnly endDate,
        Guid? excludedRentalId = null,
        CancellationToken cancellationToken = default)
    {
        var car = await Context.Set<Car>()
            .Include(c => c.Rentals)
            .Include(c => c.Services)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == carId, cancellationToken);

        if (car == null || car.Status != CarStatus.Available)
        {
            return false;
        }

        // Check for overlapping rentals
        var hasOverlappingRentals = car.Rentals.Any(r =>
            r.Status == RentalStatus.Active &&
            r.RentalPeriod.StartDate < endDate &&
            r.RentalPeriod.EndDate > startDate &&
            (excludedRentalId == null || r.Id != excludedRentalId.Value));

        if (hasOverlappingRentals)
        {
            return false;
        }

        // Check for overlapping services
        var hasOverlappingServices = car.Services.Any(s =>
            s.OverlapsWith(startDate, endDate));

        if (hasOverlappingServices)
        {
            return false;
        }

        return true;
    }

    public async Task<List<Car>> GetNextCarServicesAsync(int nextDays)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var futureDate = today.AddDays(nextDays);

        var cars = await Context.Set<Car>()
            .Include(c => c.Services)
            .AsNoTracking()
            .Where(c => c.Services.Any(s => s.Date >= today && s.Date <= futureDate))
            .ToListAsync();

        return cars;
    }
}