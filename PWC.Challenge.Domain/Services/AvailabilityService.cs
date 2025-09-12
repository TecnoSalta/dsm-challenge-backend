using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Domain.Interfaces;

namespace PWC.Challenge.Domain.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly ICarRepository _carRepository;
    private readonly IRentalRepository _rentalRepository;
    private readonly IServiceRepository _serviceRepository;

    public AvailabilityService(
        ICarRepository carRepository,
        IRentalRepository rentalRepository,
        IServiceRepository serviceRepository)
    {
        _carRepository = carRepository;
        _rentalRepository = rentalRepository;
        _serviceRepository = serviceRepository;
    }

    public async Task<bool> IsCarAvailableAsync(Guid carId, DateOnly startDate, DateOnly endDate, Guid? excludedRentalId = null)
    {
        var car = await _carRepository.GetByIdAsync(carId);
        if (car == null || car.Status != CarStatus.Available)
            return false;

        // 1. Verificar servicios programados
        if (await HasScheduledServicesAsync(carId, startDate, endDate))
            return false;

        // 2. Verificar rentals existentes
        if (await HasOverlappingRentalsAsync(carId, startDate, endDate, excludedRentalId))
            return false;

        // 3. Verificar regla: auto no disponible día siguiente al rental
        if (await HasRentalEndingBeforeStartAsync(carId, startDate))
            return false;

        return true;
    }

    public async Task<List<Car>> GetAvailableCarsAsync(DateOnly startDate, DateOnly endDate, string? carType = null, string? model = null)
    {
        var allCars = await _carRepository.GetAllAsync();
        var availableCars = new List<Car>();

        foreach (var car in allCars)
        {
            if (!PassesFilters(car, carType, model))
                continue;

            if (await IsCarAvailableAsync(car.Id, startDate, endDate))
            {
                availableCars.Add(car);
            }
        }

        return availableCars;
    }

    private async Task<bool> HasScheduledServicesAsync(Guid carId, DateOnly startDate, DateOnly endDate)
    {
        return await _serviceRepository.HasScheduledServicesAsync(carId, startDate, endDate);
    }

    private async Task<bool> HasOverlappingRentalsAsync(Guid carId, DateOnly startDate, DateOnly endDate, Guid? excludedRentalId = null)
    {
        return await _rentalRepository.HasOverlappingRentalsAsync(carId, startDate, endDate, excludedRentalId);
    }

    private async Task<bool> HasRentalEndingBeforeStartAsync(Guid carId, DateOnly startDate)
    {
        // Regla: Auto no disponible si tiene un rental que termina el día anterior
        var previousDay = startDate.AddDays(-1);
        var rentalsEndingPreviousDay = await _rentalRepository.GetOverlappingRentalsAsync(
            carId, previousDay, previousDay);

        return rentalsEndingPreviousDay.Any(r => r.EndDate == previousDay);
    }

    private bool PassesFilters(Car car, string? carType, string? model)
    {
        if (!string.IsNullOrEmpty(carType) && !car.Type.Equals(carType, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrEmpty(model) && !car.Model.Contains(model, StringComparison.OrdinalIgnoreCase))
            return false;

        return car.Status == CarStatus.Available;
    }
}