using PWC.Challenge.Application.Common;
using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Application.Services
{
    public interface ICustomerService : IBaseService<Customer, CustomerDto>
    {
    }
}