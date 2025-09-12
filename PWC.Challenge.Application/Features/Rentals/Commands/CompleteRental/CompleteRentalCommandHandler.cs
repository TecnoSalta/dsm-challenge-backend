using MediatR;
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Application.Features.Rentals.Commands.CompleteRental;
using PWC.Challenge.Application.Features.Rentals.Commands.CompleteRental.Services;
using PWC.Challenge.Application.Features.Rentals.Commands.UpdateRental.Services;

namespace PWC.Challenge.Application.Features.Rentals.Commands.UpdateRental;

public class CompleteRentalCommandHandler(ICompleteRentalService rentalService)
        : IRequestHandler<CompleteRentalCommand, CompletedRentalDto>
{
    private readonly ICompleteRentalService _rentalService = rentalService;

    public async Task<CompletedRentalDto> Handle(CompleteRentalCommand request, CancellationToken cancellationToken)
    {
        DateOnly today  = DateOnly.FromDateTime(DateTime.UtcNow);
        return await _rentalService.CompleteAsync(
            request.RentalId, today, cancellationToken);
    }
}
