using MediatR;

namespace PWC.Challenge.Application.Features.Cars.Queries.GetCarMetadata;

public class GetCarMetadataQuery : IRequest<List<CarMetadataDto>>
{
}