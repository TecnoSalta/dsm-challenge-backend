using PWC.Challenge.Common.CQRS;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Application.Features.Cars.Queries.GetNextCarServices;

public record GetNextCarServicesQuery() : IQuery<IReadOnlyList<Car>>;
