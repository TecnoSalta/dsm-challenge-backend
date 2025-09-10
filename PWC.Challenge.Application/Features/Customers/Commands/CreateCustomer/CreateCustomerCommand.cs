using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Common.CQRS;

namespace PWC.Challenge.Application.Features.Customers.Commands.CreateCustomer
{
    public record CreateCustomerCommand(CustomerDto Customer) : ICommand<Guid>;
}
