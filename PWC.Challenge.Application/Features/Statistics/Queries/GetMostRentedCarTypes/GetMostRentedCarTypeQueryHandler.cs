using MediatR;
using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Application.Services;

namespace PWC.Challenge.Application.Features.Statistics.Queries.GetMostRentedCarTypes;

public class GetMostRentedCarTypeQueryHandler(
    IStatisticService statisticService) : IRequestHandler<GetMostRentedCarTypeQuery, List<CarTypeRentalCountDto>>
{
    public async Task<List<CarTypeRentalCountDto>> Handle(GetMostRentedCarTypeQuery query, CancellationToken cancellationToken)
    {
        var results = await statisticService.GetMostRentedCarTypesAsync(query.StartDate, query.EndDate);

        return results;
    }
}
