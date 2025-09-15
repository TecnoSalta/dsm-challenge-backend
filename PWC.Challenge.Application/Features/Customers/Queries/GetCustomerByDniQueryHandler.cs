using MediatR;
using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Domain.Interfaces;
using Mapster;

namespace PWC.Challenge.Application.Features.Customers.Queries;

public class GetCustomerByDniQueryHandler : IRequestHandler<GetCustomerByDniQuery, CustomerDto?>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByDniQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerDto?> Handle(GetCustomerByDniQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByDniAsync(request.Dni);

        return customer?.Adapt<CustomerDto>();
    }
}
