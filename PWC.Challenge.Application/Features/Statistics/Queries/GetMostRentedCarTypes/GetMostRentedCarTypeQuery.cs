using MediatR;
using PWC.Challenge.Application.Dtos;

namespace PWC.Challenge.Application.Features.Statistics.Queries.GetMostRentedCarTypes;

public record GetMostRentedCarTypeQuery(DateOnly? StartDate = null, DateOnly? EndDate = null) : IRequest<List<CarTypeRentalCountDto>>;
