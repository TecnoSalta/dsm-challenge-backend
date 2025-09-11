using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Common.CQRS;

namespace PWC.Challenge.Application.Features.Rentals.Commands.UpdateRental;

public record UpdateRentalCommand(
    Guid RentalId,
    UpdateRentalDto Payload
) : ICommand<UpdatedRentalDto>;