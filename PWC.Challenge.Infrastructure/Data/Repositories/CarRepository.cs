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
        string? carModel = null,
        CancellationToken cancellationToken = default)
    {
        var availableCars = await Context.Set<Car>()
            .Where(car => car.Status == CarStatus.Available)
            
            // Filtros opcionales por tipo y modelo
            .Where(car => carType == null || car.Type == carType)
            .Where(car => carModel == null || car.Model == carModel)
            
            // Excluir autos que tienen servicios programados en el período
            .Where(car => !car.Services.Any(service => 
                service.Date <= endDate && 
                service.Date.AddDays(service.DurationDays) >= startDate))
            
            // Excluir autos que tienen alquileres activos/reservados que se solapan
            .Where(car => !car.Rentals.Any(rental => 
                (rental.Status == RentalStatus.Active || rental.Status == RentalStatus.Reserved) &&
                rental.RentalPeriod.StartDate <= endDate && 
                rental.RentalPeriod.EndDate >= startDate))
            .ToListAsync(cancellationToken);

        return availableCars;
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