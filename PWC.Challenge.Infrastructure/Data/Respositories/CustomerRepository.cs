using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace PWC.Challenge.Infrastructure.Data.Respositories;


public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context) : base(context)
    {
    }

 

   
}