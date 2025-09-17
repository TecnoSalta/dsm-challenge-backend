using Mapster;
using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Exceptions;
using PWC.Challenge.Domain.Interfaces;
using PWC.Challenge.Application.Services;
using PWC.Challenge.Application.Exceptions; // This is for ICarService

namespace PWC.Challenge.Application.Services;

public class CarService : ICarService
{
    private readonly ICarRepository _carRepository;

    public CarService(ICarRepository carRepository)
    {
        _carRepository = carRepository;
    }

    public async Task<IEnumerable<AvailableCarDto>> GetAvailableCarsAsync(DateOnly startDate, DateOnly endDate, string? carType, string? carModel)
    {
        var availableCars = await _carRepository.GetAvailableCarsAsync(startDate, endDate, carType, carModel);
        return availableCars.Adapt<IEnumerable<AvailableCarDto>>();
    }

    public async Task<CarAvailabilityDto> GetCarAvailabilityAsync(Guid carId, DateOnly startDate, DateOnly endDate)
    {
        // Fetch the car with its rentals and services to perform domain logic
        var car = await _carRepository.GetByIdAsync(carId, asNoTracking: false); // Need to ensure rentals and services are loaded

        if (car == null)
        {
            throw new EntityNotFoundException<Guid>(nameof(Car), carId);
        }

        var isAvailable = car.IsAvailableForPeriod(startDate, endDate);

        // Map the car to CarAvailabilityDto and set the availability flag
        var carAvailabilityDto = car.Adapt<CarAvailabilityDto>();
        carAvailabilityDto = carAvailabilityDto with { IsAvailableForPeriod = isAvailable }; // Using 'with' for record immutability

        return carAvailabilityDto;
    }
}