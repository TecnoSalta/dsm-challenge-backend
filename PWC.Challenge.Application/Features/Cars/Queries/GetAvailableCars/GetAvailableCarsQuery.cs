using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Common.CQRS;

namespace PWC.Challenge.Application.Features.Cars.Queries.GetAvailableCars;
public record GetAvailableCarsQuery(AvailabilityQueryDto Filter)
    : IQuery<IReadOnlyList<AvailableCarDto>>;
