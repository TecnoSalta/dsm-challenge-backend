using MediatR;
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Common.CQRS;

namespace PWC.Challenge.Application.Features.Rentals.Commands.CompleteRental;

public record CompleteRentalCommand(Guid RentalId) :  ICommand<CompletedRentalDto>;