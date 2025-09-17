using PWC.Challenge.Common.CQRS;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Interfaces;
namespace PWC.Challenge.Application.Features.Cars.Queries.GetNextCarServices;

public class GetAvailableCarsQueryHandler(
    ICarRepository carRepository) : IQueryHandler<GetNextCarServicesQuery, IReadOnlyList<Car>>
{
    public async Task<IReadOnlyList<Car>> Handle(GetNextCarServicesQuery request, CancellationToken cancellationToken)
    {
        var results = await carRepository.GetNextCarServicesAsync(14);

        return results;
    }
}