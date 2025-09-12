using Microsoft.Extensions.Logging;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Interfaces;
using PWC.Challenge.Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<Car>> GetAvailableCarsAsync(DateTime startDate, DateTime endDate, string carType = null, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"availability_cars:{startDate:yyyyMMdd}:{endDate:yyyyMMdd}:{carType ?? "all"}";

            try
            {
                // Intentar obtener del cache
                var cachedResult = await _cacheService.GetAsync<IEnumerable<Car>>(cacheKey, cancellationToken);
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
            var result = await _decorated.GetAvailableCarsAsync(startDate, endDate, carType, cancellationToken);

            try
            {
                // Guardar en cache
                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromSeconds(30), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error saving to cache for availability");
            }

            return result;
        }

        public async Task<bool> IsCarAvailableAsync(int carId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"car_availability:{carId}:{startDate:yyyyMMdd}:{endDate:yyyyMMdd}";

            try
            {
                var cachedResult = await _cacheService.GetAsync<bool?>(cacheKey, cancellationToken);
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

            var result = await _decorated.IsCarAvailableAsync(carId, startDate, endDate, cancellationToken);

            try
            {
                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromSeconds(30), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error saving to cache for car availability");
            }

            return result;
        }

        public async Task InvalidateAvailabilityCache(int? carId = null, DateTime? startDate = null, DateTime? endDate = null)
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
    }
}