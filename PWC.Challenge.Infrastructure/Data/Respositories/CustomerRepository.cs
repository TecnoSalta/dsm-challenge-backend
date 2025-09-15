using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Interfaces;
using PWC.Challenge.Infrastructure.Data.Common;

namespace PWC.Challenge.Infrastructure.Data.Repositories;

public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Customer?> GetByDniAsync(string dni, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Customer>()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Dni == dni.Trim(), cancellationToken);
    }
}