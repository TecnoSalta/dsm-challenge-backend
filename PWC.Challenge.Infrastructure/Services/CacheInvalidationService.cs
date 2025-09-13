using Microsoft.Extensions.Logging;
using PWC.Challenge.Application.Services;

namespace PWC.Challenge.Infrastructure.Services
{
    public interface ICacheInvalidationService
    {
        Task InvalidateAvailabilityCacheAsync(int? carId = null, CancellationToken cancellationToken = default);
        Task InvalidateCarCacheAsync(int carId, CancellationToken cancellationToken = default);
    }

    public class CacheInvalidationService : ICacheInvalidationService
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<CacheInvalidationService> _logger;

        public CacheInvalidationService(ICacheService cacheService, ILogger<CacheInvalidationService> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task InvalidateAvailabilityCacheAsync(int? carId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                if (carId.HasValue)
                {
                    await _cacheService.RemoveByPatternAsync($"car_availability:{carId.Value}:*");
                    await _cacheService.RemoveByPatternAsync($"available_cars:*:car:{carId.Value}");
                    _logger.LogInformation("Invalidated availability cache for car {CarId}", carId.Value);
                }
                else
                {
                    await _cacheService.RemoveByPatternAsync("available_cars:*");
                    await _cacheService.RemoveByPatternAsync("car_availability:*");
                    await _cacheService.RemoveByPatternAsync("availability_cars:*");
                    _logger.LogInformation("Invalidated all availability cache");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating availability cache");
            }
        }

        public async Task InvalidateCarCacheAsync(int carId, CancellationToken cancellationToken = default)
        {
            try
            {
                await _cacheService.RemoveByPatternAsync($"car:{carId}:*");
                _logger.LogInformation("Invalidated cache for car {CarId}", carId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating car cache");
            }
        }
    }
}