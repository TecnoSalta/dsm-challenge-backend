using MediatR;
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Application.Features.Rentals.Commands.CompleteRental;
using PWC.Challenge.Application.Features.Rentals.Commands.UpdateRental.Services;

namespace PWC.Challenge.Application.Features.Rentals.Commands.UpdateRental;

public class CompleteRentalCommandHandler(IRentalService rentalService)
        : IRequestHandler<CompleteRentalCommand, CompletedRentalDto>
{
    private readonly IRentalService _rentalService = rentalService;

    public async Task<CompletedRentalDto> Handle(CompleteRentalCommand request, CancellationToken cancellationToken)
    {
        return await _rentalService.CompleteRentalAsync(
            request.RentalId,cancellationToken
        );
    }
}
