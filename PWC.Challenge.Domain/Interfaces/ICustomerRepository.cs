

using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Domain.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer> GetByIdAsync(Guid id);
    Task<Customer> GetByIdentityIdAsync(string identityId);
    Task<bool> ExistsByIdentityIdAsync(string identityId);
    Task AddAsync(Customer customer);
}