using MediatR;
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Application.Features.Rentals.Commands.UpdateRental;
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
    /// <summary>
    /// Dada una reserva existente, actualiza sus fechas y/o coche.
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<UpdatedRentalDto> Handle(
        UpdateRentalCommand cmd,
        CancellationToken cancellationToken)
    {
        // Delegamos toda la lógica de negocio al service
        return await _rentalService.UpdateRentalAsync(
            cmd.RentalId,
            cmd.Payload.NewStartDate,
            cmd.Payload.NewEndDate,
            cmd.Payload.NewCarId,
            cancellationToken
        );
    }
}
