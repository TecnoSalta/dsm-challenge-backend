using MediatR;
using PWC.Challenge.Application.Dtos.Auth;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Interfaces;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PWC.Challenge.Application.Features.Auth.Commands.RegisterCustomer
{
    public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, RegisterResponseDto>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<RegisterResponseDto> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
        {
        
        var customer = new Customer(
                Guid.NewGuid(), // Generate a new GUID for the customer ID
                request.Dni,
                request.FullName,
                request.Address,
                request.Email
            );

            customer.UserId = request.UserId; // Link to IdentityUser

            await _customerRepository.AddAsync(customer, false, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RegisterResponseDto
            (
                request.UserId,
                customer.Id, // Return the newly created customer ID
                "Customer" // Default role
            );
        }
    }
}
