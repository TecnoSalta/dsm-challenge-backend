using MediatR;
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Application.Features.Commands.Rentals.UpdateReservation.Services;

namespace PWC.Challenge.Application.Features.Commands.Rentals.UpdateReservation;

public class UpdateReservationCommandHandler
    : IRequestHandler<UpdateReservationCommand, UpdatedReservationDto>
{
    private readonly IRentalService _rentalService;

    public UpdateReservationCommandHandler(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    public async Task<UpdatedReservationDto> Handle(
        UpdateReservationCommand cmd,
        CancellationToken ct)
    {
        // Delegamos toda la lógica de negocio al service
        return await _rentalService.UpdateReservationAsync(
            cmd.ReservationId,
            cmd.Payload.NewStartDate,
            cmd.Payload.NewEndDate,
            cmd.Payload.NewCarId,
            ct
        );
    }
}
