using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Interfaces;


namespace PWC.Challenge.Infrastructure.Data.Respositories;


public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context) : base(context)
    {
    }

 

   
}