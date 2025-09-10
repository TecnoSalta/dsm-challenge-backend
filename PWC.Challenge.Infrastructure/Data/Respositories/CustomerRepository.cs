using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Interfaces;
using PWC.Challenge.Infrastructure.Data.Common;


namespace PWC.Challenge.Infrastructure.Data.Respositories;


public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context) : base(context)
    {
    }

 

   
}