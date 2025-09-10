using Mapster;
using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Application.Mappings.Mapster;



public class EventItemMapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CustomerDto, Customer>();
        config.NewConfig<Customer, CustomerDto>();

    }
}
