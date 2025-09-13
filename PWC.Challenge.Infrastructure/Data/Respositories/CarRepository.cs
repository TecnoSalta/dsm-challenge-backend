﻿using Microsoft.EntityFrameworkCore;
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

    public async Task<List<Car>> GetAvailableCarsAsync(DateOnly startDate, DateOnly endDate, string? carType = null, string? model = null)
    {
        var query = Context.Set<Car>()
            .Include(c => c.Services)
            .Where(c => c.Status == CarStatus.Available)
            .Where(c => !c.Services.Any(s => s.OverlapsWith(startDate, endDate)))
            .Where(c => !Context.Set<Rental>()
                .Any(r => r.CarId == c.Id &&
                         r.Status == RentalStatus.Active &&
                         r.RentalPeriod.StartDate < endDate &&
                         r.RentalPeriod.EndDate > startDate));

        if (!string.IsNullOrEmpty(carType))
            query = query.Where(c => c.Type == carType);

        if (!string.IsNullOrEmpty(model))
            query = query.Where(c => c.Model.Contains(model));

        return await query.ToListAsync();
    }

    public async Task<bool> IsCarAvailableAsync(Guid carId, DateOnly startDate, DateOnly endDate, Guid? excludedRentalId = null)
    {
        var car = await Context.Set<Car>()
            .Include(c => c.Services)
            .FirstOrDefaultAsync(c => c.Id == carId);

        if (car == null || car.Status != CarStatus.Available)
            return false;

        // Check services
        if (car.Services.Any(s => s.OverlapsWith(startDate, endDate)))
            return false;

        // Check rentals
        var rentalQuery = Context.Set<Rental>()
            .Where(r => r.CarId == carId &&
                       r.Status == RentalStatus.Active &&
                       r.RentalPeriod.StartDate < endDate &&
                       r.RentalPeriod.EndDate > startDate);

        if (excludedRentalId.HasValue)
            rentalQuery = rentalQuery.Where(r => r.Id != excludedRentalId.Value);

        return !await rentalQuery.AnyAsync();
    }
}