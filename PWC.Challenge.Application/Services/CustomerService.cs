using PWC.Challenge.Application.Common;
using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Application.Services;

public class CustomerService : BaseService<Customer, CustomerDto>, ICustomerService
{
    public CustomerService(IBaseRepository<Customer> repository) : base(repository)
    {
    }
}
