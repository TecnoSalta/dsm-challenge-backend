using MediatR;
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Application.Features.Rentals.Commands.UpdateRental.Services;

namespace PWC.Challenge.Application.Features.Rentals.Commands.CancelRental;

// Cambia el tipo de respuesta de IRequestHandler y asegúrate de que CancelRentalCommand implemente IRequest<CancelRentalDto>
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
        // Delegamos toda la lógica de negocio al service
        var updatedRental = await _rentalService.UpdateRentalAsync(
            request.ReservationId,
            request.Payload.NewStartDate,
            request.Payload.NewEndDate,
            request.Payload.NewCarId,
            cancellationToken
        );

        return new CancelRentalDto(updatedRental.RentalId);
    }
}
