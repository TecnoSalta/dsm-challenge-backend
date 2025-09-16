using MediatR;
using PWC.Challenge.Application.Dtos.Profile;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;
using Mapster;

namespace PWC.Challenge.Application.Features.Profile.Queries;

public class GetProfileByUserIdQueryHandler : IRequestHandler<GetProfileByUserIdQuery, ProfileDto>
{
    private readonly IBaseRepository<Customer> _customerRepository;

    public GetProfileByUserIdQueryHandler(IBaseRepository<Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ProfileDto> Handle(GetProfileByUserIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetSingleAsync(c => c.UserId == request.UserId, null, false, cancellationToken);

        if (customer == null)
        {
            return null; // Or throw a specific exception if preferred
        }

        return customer.Adapt<ProfileDto>();
    }
}
