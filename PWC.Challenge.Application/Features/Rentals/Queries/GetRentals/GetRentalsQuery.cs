using MediatR;
using PWC.Challenge.Application.Dtos.Rentals;
using System.Collections.Generic;

namespace PWC.Challenge.Application.Features.Rentals.Queries.GetRentals;

public record GetRentalsQuery() : IRequest<IReadOnlyList<RentalDto>>;
