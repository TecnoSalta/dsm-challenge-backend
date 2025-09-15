using MediatR;
using PWC.Challenge.Domain.Interfaces;
using PWC.Challenge.Domain.Entities;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PWC.Challenge.Application.Services; // Added for ICacheService
using System; // Added for TimeSpan

namespace PWC.Challenge.Application.Features.Cars.Queries.GetCarMetadata;

public class GetCarMetadataQueryHandler : IRequestHandler<GetCarMetadataQuery, List<CarMetadataDto>>
{
    private readonly ICarRepository _carRepository;
    private readonly ICacheService _cacheService; // Injected ICacheService
    private const string CarMetadataCacheKey = "CarMetadata"; // Define a cache key

    public GetCarMetadataQueryHandler(ICarRepository carRepository, ICacheService cacheService)
    {
        _carRepository = carRepository;
        _cacheService = cacheService; // Initialize ICacheService
    }

    public async Task<List<CarMetadataDto>> Handle(GetCarMetadataQuery request, CancellationToken cancellationToken)
    {
        // Use GetOrCreateAsync to fetch from cache or database
        var metadata = await _cacheService.GetOrCreateAsync(
            CarMetadataCacheKey,
            async () =>
            {
                var allCars = await _carRepository.GetAllAsync(asNoTracking: true, cancellationToken);

                return allCars
                    .GroupBy(car => car.Type)
                    .Select(group => new CarMetadataDto
                    {
                        Type = group.Key,
                        Models = group.Select(car => car.Model).Distinct().ToList()
                    })
                    .ToList();
            },
            TimeSpan.FromMinutes(5), // 5-minute TTL
            cancellationToken
        );

        return metadata;
    }
}