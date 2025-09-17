using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Application.Services;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Infrastructure.Data;

namespace PWC.Challenge.Infrastructure.Services;

public class StatisticService : IStatisticService
{
    private readonly ApplicationDbContext _context;

    public StatisticService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CarTypeRentalCountDto>> GetMostRentedCarTypesAsync(DateOnly? startDate = null, DateOnly? endDate = null)
    {
        var query = _context.Rentals
            .AsNoTracking()
            .Include(r => r.Car)
            .Where(r => r.Status == RentalStatus.Completed &&
                (!startDate.HasValue || r.RentalPeriod.StartDate >= startDate.Value) &&
                (!endDate.HasValue || r.RentalPeriod.EndDate <= endDate.Value));

        var total = query.Count();

        var results = await query
            .GroupBy(r => r.Car.Type)
            .Select(g => new CarTypeRentalCountDto
            {
                CarType = g.Key,
                RentalCount = g.Count(),
                Percentage = (decimal)g.Count() / total * 100
            })
            .OrderByDescending(x => x.RentalCount)
            .Take(5)
            .ToListAsync();

        return results;
    }
}
