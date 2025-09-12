using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Domain.Interfaces;

namespace PWC.Challenge.Domain.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly ICarRepository _carRepository;
    private readonly IRentalRepository _rentalRepository;

    public AvailabilityService(ICarRepository carRepository, IRentalRepository rentalRepository)
    {
        _carRepository = carRepository;
        _rentalRepository = rentalRepository;
    }

    public async Task<bool> IsCarAvailableAsync(Guid carId, DateOnly startDate, DateOnly endDate, Guid? excludedRentalId = null)
    {
        var car = await _carRepository.GetByIdAsync(carId);
        if (car == null) return false;

        // Check if car is in maintenance
        if (car.Status == CarStatus.InMaintenance)
            return false;

        // Check service periods
        if (car.IsInServicePeriod(startDate, endDate))
            return false;

        // Check existing rentals (excluding current rental if updating)
        var overlappingRentals = await _rentalRepository.GetOverlappingRentalsAsync(carId, startDate, endDate, excludedRentalId);
        return !overlappingRentals.Any();
    }

    public async Task<List<Car>> GetAvailableCarsAsync(DateOnly startDate, DateOnly endDate, string? carType = null, string? model = null)
    {
        var allCars = await _carRepository.GetAllAsync();
        var availableCars = new List<Car>();

        foreach (var car in allCars)
        {
            if (car.Status == CarStatus.InMaintenance)
                continue;

            if (car.IsInServicePeriod(startDate, endDate))
                continue;

            // Check if any overlapping rentals exist
            var hasOverlappingRentals = await _rentalRepository.HasOverlappingRentalsAsync(car.Id, startDate, endDate);
            if (!hasOverlappingRentals)
            {
                // Apply filters if specified
                if (carType != null && !car.Type.Equals(carType, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (model != null && !car.Model.Equals(model, StringComparison.OrdinalIgnoreCase))
                    continue;

                availableCars.Add(car);
            }
        }

        return availableCars;
    }
}