using MediatR;
using PWC.Challenge.Application.Dtos.Rentals;

namespace PWC.Challenge.Application.Features.Rentals.Commands.CancelRental;

public record CancelRentalCommand(Guid RentalId) : IRequest<CancelRentalDto>;