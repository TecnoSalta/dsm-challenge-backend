using MediatR;
using PWC.Challenge.Application.Dtos.Rentals;

namespace PWC.Challenge.Application.Features.Rentals.Commands.CreateRental;

public record CreateRentalCommand(CreateRentalRequestDto CreateDto) : IRequest<CreatedRentalDto>;