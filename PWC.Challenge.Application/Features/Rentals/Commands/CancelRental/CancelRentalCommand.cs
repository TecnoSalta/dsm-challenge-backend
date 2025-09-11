using MediatR;
using PWC.Challenge.Application.Dtos.Rentals;

namespace PWC.Challenge.Application.Features.Rentals.Commands.CancelRental;

// Cambia la implementación de CancelRentalCommand para que implemente IRequest<CancelRentalDto>
public record CancelRentalCommand : IRequest<CancelRentalDto>
{
    public Guid ReservationId { get; init; }
    public UpdateRentalDto Payload { get; init; }
}