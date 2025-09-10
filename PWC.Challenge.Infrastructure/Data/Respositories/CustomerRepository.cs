using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace PWC.Challenge.Infrastructure.Data.Respositories;


public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Customer> GetByIdentityIdAsync(string identityId)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.IdentityId == identityId);
    }

    public async Task<bool> ExistsByIdentityIdAsync(string identityId)
    {
        return await _dbSet.AnyAsync(c => c.IdentityId == identityId);
    }

    public async Task<IEnumerable<Customer>> GetCustomersWithRentalsAsync()
    {
        return await _dbSet
           // .Include(c => c.Rentals)
            .ToListAsync();
    }

    public Task AddAsync(Customer customer)
    {
        throw new NotImplementedException();
    }
}