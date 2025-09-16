using MediatR;
using PWC.Challenge.Application.Dtos.Profile;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;
using Mapster;

namespace PWC.Challenge.Application.Features.Profile.Queries;

public class GetProfileByCustomerIdQueryHandler : IRequestHandler<GetProfileByCustomerIdQuery, ProfileDto>
{
    private readonly IBaseRepository<Customer> _customerRepository;

    public GetProfileByCustomerIdQueryHandler(IBaseRepository<Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ProfileDto> Handle(GetProfileByCustomerIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, false, cancellationToken);

        if (customer == null)
        {
            return null; // Or throw a specific exception if preferred
        }

        return customer.Adapt<ProfileDto>();
    }
}
