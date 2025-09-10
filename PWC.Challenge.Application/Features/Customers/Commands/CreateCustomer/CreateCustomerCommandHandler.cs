using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Application.Exceptions;
using PWC.Challenge.Application.Services;
using PWC.Challenge.Common.CQRS;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler(ICustomerService service) : ICommandHandler<CreateCustomerCommand, Guid>
{
    public async Task<Guid> Handle(CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        try
        {
            CustomerDto? entityDto;
                
            entityDto = await service.GetByIdAsync(command.CustomerDto.Id, true, cancellationToken);
            if (entityDto is not null)
                throw new EntityAlreadyExistsException<Guid>(typeof(Customer).Name, command.CustomerDto.Id);
            
            entityDto = await service.AddAsync(command.CustomerDto, true, cancellationToken);
            return entityDto.Id;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
