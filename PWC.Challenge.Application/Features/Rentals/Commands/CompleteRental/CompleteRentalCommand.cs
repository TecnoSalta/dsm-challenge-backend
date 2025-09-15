using MediatR;
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Common.CQRS;
using System;

namespace PWC.Challenge.Application.Features.Rentals.Commands.CompleteRental;

public record CompleteRentalCommand(Guid RouteId, Guid BodyRentalId) : ICommand<CompletedRentalDto>;