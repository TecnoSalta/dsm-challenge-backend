using PWC.Challenge.Domain.Interfaces;
using PWC.Challenge.Domain.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PWC.Challenge.Infrastructure.Services;

public class RentalAvailabilityService : IRentalAvailabilityService
{
    private readonly ICarRepository _carRepository;
    private readonly IRentalRepository _rentalRepository;

    public RentalAvailabilityService(ICarRepository carRepository, IRentalRepository rentalRepository)
    {
        _carRepository = carRepository;
        _rentalRepository = rentalRepository;
    }

    public async Task<bool> IsCarAvailableAsync(Guid carId, DateOnly startDate, DateOnly endDate, Guid? excludedRentalId = null, CancellationToken cancellationToken = default)
    {
        // Check if the car is in service during the rental period
        var car = await _carRepository.GetByIdAsync(carId, asNoTracking: true, cancellationToken);
        if (car == null)
        {
            // If car doesn't exist, it's not available
            return false;
        }

        // This check should ideally be part of the Car entity's behavior
        // if (car.IsInServicePeriod(startDate, endDate))
        // {
        //     return false;
        // }

        // Check for overlapping rentals
        var hasOverlappingRentals = await _rentalRepository.HasOverlappingRentalsAsync(carId, startDate, endDate, excludedRentalId);
        if (hasOverlappingRentals)
        {
            return false;
        }

        return true;
    }
}