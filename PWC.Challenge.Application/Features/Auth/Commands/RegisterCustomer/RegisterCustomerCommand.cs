using MediatR;
using PWC.Challenge.Application.Dtos.Auth;

namespace PWC.Challenge.Application.Features.Auth.Commands.RegisterCustomer
{
    public record RegisterCustomerCommand(string FullName,string Dni,string Address, string Email, Guid UserId) : IRequest<RegisterResponseDto>;
}
