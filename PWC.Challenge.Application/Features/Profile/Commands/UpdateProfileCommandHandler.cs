using MediatR;
using PWC.Challenge.Application.Dtos.Profile;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;
using Mapster;

namespace PWC.Challenge.Application.Features.Profile.Commands;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ProfileDto>
{
    private readonly IBaseRepository<Customer> _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileCommandHandler(IBaseRepository<Customer> customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProfileDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetSingleAsync(c => c.UserId == request.UserId, null, false, cancellationToken);

        if (customer == null)
        {
            return null; // Or throw a specific exception if preferred
        }

        customer.UpdateFullName(request.FullName);
        customer.UpdateAddress(request.Address);

        await _customerRepository.UpdateAsync(customer, false, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return customer.Adapt<ProfileDto>();
    }
}
