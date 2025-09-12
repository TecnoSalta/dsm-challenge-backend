using Microsoft.Extensions.Logging;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Services;

namespace PWC.Challenge.Infrastructure.Services
{
    public class CachedAvailabilityService : IAvailabilityService
    {
        private readonly IAvailabilityService _decorated;
        private readonly ICacheService _cacheService;
        private readonly ILogger<CachedAvailabilityService> _logger;

        public CachedAvailabilityService(
            IAvailabilityService decorated,
            ICacheService cacheService,
            ILogger<CachedAvailabilityService> logger)
        {
            _decorated = decorated;
            _cacheService = cacheService;
            _logger = logger;
        }
        public async Task InvalidateAvailabilityCache(Guid? carId = null, DateOnly? startDate = null, DateOnly? endDate = null)
        {
            try
            {
                if (carId.HasValue)
                {
                    await _cacheService.RemoveByPatternAsync($"car_availability:{carId.Value}:");
                    _logger.LogInformation("Invalidated cache for car {CarId}", carId.Value);
                }
                else
                {
                    await _cacheService.RemoveByPatternAsync("availability_cars:");
                    await _cacheService.RemoveByPatternAsync("car_availability:");
                    _logger.LogInformation("Invalidated all availability cache");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating availability cache");
            }
        }

        public async Task<bool> IsCarAvailableAsync(Guid carId, DateOnly startDate, DateOnly endDate, Guid? excludedRentalId = null)
        {
            var cacheKey = $"car_availability:{carId}:{startDate:yyyyMMdd}:{endDate:yyyyMMdd}";

            try
            {
                var cachedResult = await _cacheService.GetAsync<bool?>(cacheKey);
                if (cachedResult.HasValue)
                {
                    _logger.LogDebug("Cache hit for car availability: {CacheKey}", cacheKey);
                    return cachedResult.Value;
                }

                _logger.LogDebug("Cache miss for car availability: {CacheKey}", cacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error accessing cache for car availability, falling back to service");
            }

            var result = await _decorated.IsCarAvailableAsync(carId, startDate, endDate);

            try
            {
                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromSeconds(30));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error saving to cache for car availability");
            }

            return result;
        }

        public async Task<List<Car>> GetAvailableCarsAsync(DateOnly startDate, DateOnly endDate, string? carType = null, string? model = null)
        {
            var cacheKey = $"availability_cars:{startDate:yyyyMMdd}:{endDate:yyyyMMdd}:{carType ?? "all"}";

            try
            {
                // Intentar obtener del cache - cambiar a List<Car>
                var cachedResult = await _cacheService.GetAsync<List<Car>>(cacheKey);
                if (cachedResult != null)
                {
                    _logger.LogDebug("Cache hit for availability: {CacheKey}", cacheKey);
                    return cachedResult;
                }

                _logger.LogDebug("Cache miss for availability: {CacheKey}", cacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error accessing cache for availability, falling back to service");
            }

            // Obtener del servicio real
            var result = await _decorated.GetAvailableCarsAsync(startDate, endDate, carType);

            try
            {
                // Guardar en cache
                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromSeconds(30));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error saving to cache for availability");
            }

            return result;
        }
    }
}