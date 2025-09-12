using MediatR;
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Application.Features.Rentals.Commands.UpdateRental.Services;

namespace PWC.Challenge.Application.Features.Rentals.Commands.CancelRental;

public class CancelRentalCommandHandler
    : IRequestHandler<CancelRentalCommand, CancelRentalDto>
{
    private readonly IRentalService _rentalService;

    public CancelRentalCommandHandler(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    public async Task<CancelRentalDto> Handle(CancelRentalCommand request, CancellationToken cancellationToken)
    {
        var updatedRental = await _rentalService.CancelRentalAsync(
            request.RentalId,
            cancellationToken
        );

        return new CancelRentalDto(updatedRental.RentalId);
    }
}
