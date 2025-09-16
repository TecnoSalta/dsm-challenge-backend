using MediatR;
using PWC.Challenge.Application.Dtos.Rentals;

namespace PWC.Challenge.Application.Features.Rentals.Queries.GetRentalById;

public record GetRentalByIdQuery(Guid Id) : IRequest<RentalDto>;
