using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Interfaces;
using PWC.Challenge.Infrastructure.Data.Common;

namespace PWC.Challenge.Infrastructure.Data.Repositories;

public class ServiceRepository : BaseRepository<Service>, IServiceRepository
{
    public ServiceRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> HasScheduledServicesAsync(Guid carId, DateOnly startDate, DateOnly endDate)
    {
        return await Context.Set<Service>()
            .AnyAsync(s => s.CarId == carId && s.OverlapsWith(startDate, endDate));
    }

    public async Task<List<Service>> GetScheduledServicesAsync(Guid carId, DateOnly startDate, DateOnly endDate)
    {
        return await Context.Set<Service>()
            .Where(s => s.CarId == carId && s.OverlapsWith(startDate, endDate))
            .ToListAsync();
    }
}