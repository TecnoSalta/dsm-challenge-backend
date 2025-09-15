using MediatR;
using System;

namespace PWC.Challenge.Application.Features.Rentals.Commands.CreateRental;

public record CreateRentalCommand(Guid CustomerId, Guid CarId, DateOnly StartDate, DateOnly EndDate) : IRequest<CreatedRentalDto>;