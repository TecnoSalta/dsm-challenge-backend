using Microsoft.Extensions.Logging;
using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Application.Interfaces;
using PWC.Challenge.Application.Services;
using PWC.Challenge.Common.CQRS;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Domain.Interfaces;
using PWC.Challenge.Domain.Services;


namespace PWC.Challenge.Application.Features.Cars.Queries.GetAvailableCars
{
    public class GetAvailableCarsQueryHandler(
        ICarRepository carRepository,
        IAvailabilityService availabilityService,
        ICacheService cacheService,
        IClock clock,
        ILogger<GetAvailableCarsQueryHandler> logger)
                : IQueryHandler<GetAvailableCarsQuery, IReadOnlyList<AvailableCarDto>>
    {
        private readonly ICarRepository _carRepository = carRepository;
        private readonly IAvailabilityService _availabilityService = availabilityService;
        private readonly ICacheService _cacheService = cacheService;
        private readonly IClock _clock = clock;
        private readonly ILogger<GetAvailableCarsQueryHandler> _logger = logger;

        public async Task<IReadOnlyList<AvailableCarDto>> Handle(
            GetAvailableCarsQuery request,
            CancellationToken ct)
        {
            var (startDate, endDate, carType, model) = request.Filter;

            // Validar fechas
            ValidateDates(startDate, endDate);

            // Generar clave de cache
            var cacheKey = GenerateCacheKey(startDate, endDate, carType, model);

            try
            {
                // Intentar obtener del cache
                var cachedResult = await _cacheService.GetAsync<IReadOnlyList<AvailableCarDto>>(cacheKey, ct);
                if (cachedResult != null)
                {
                    _logger.LogDebug("Cache hit for available cars: {CacheKey}", cacheKey);
                    return cachedResult;
                }

                _logger.LogDebug("Cache miss for available cars: {CacheKey}", cacheKey);
            }
            catch (Exception ex)
            {
            _logger.LogWarning(ex, "Error accessing cache for key: {CacheKey}, falling back to database", cacheKey);
                // Continuar con la consulta a la base de datos si el cache falló
            }

            // Obtener coches disponibles directamente del repositorio
            var availableCars = await _carRepository.GetAvailableCarsAsync(startDate, endDate, carType, model, ct);

            // Ensure availableCars is not null before proceeding
            if (availableCars == null)
            {
                availableCars = new List<Car>(); // Initialize as an empty list
            }

            var result = availableCars
                .Select(c => new AvailableCarDto(c.Id, c.Type, c.Model, c.DailyRate, c.LicensePlate, c.Status.ToString()))
                .ToList();

            try
            {
                // Intentar guardar en cache con expiración de 30 segundos
                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromSeconds(30), ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error saving to cache for key: {CacheKey}", cacheKey);
                // No lanzar excepción, solo loggear el error
            }

            return result;
        }

        private string GenerateCacheKey(DateOnly startDate, DateOnly endDate, string? carType, string? model)
        {
            return $"available_cars:{startDate:yyyyMMdd}:{endDate:yyyyMMdd}:{carType ?? "all"}:{model ?? "all"}";
        }

        private void ValidateDates(DateOnly startDate, DateOnly endDate)
        {
            if (startDate >= endDate)
                throw new ArgumentException("End date must be after start date");

            if (endDate.DayNumber - startDate.DayNumber < 1)
                throw new ArgumentException("Rental period must be at least 1 day");

            if (startDate < DateOnly.FromDateTime(_clock.UtcNow))
                throw new ArgumentException("Start date cannot be in the past");
        }
    }
}
