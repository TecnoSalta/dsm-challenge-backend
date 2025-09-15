using MediatR;
using PWC.Challenge.Domain.Interfaces;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Domain.ValueObjects;
using System;
using System.Threading;
using System.Threading.Tasks;
using PWC.Challenge.Application.Exceptions;
using PWC.Challenge.Domain.Services;
using PWC.Challenge.Common.Exceptions; // Added for IRentalAvailabilityService

namespace PWC.Challenge.Application.Features.Rentals.Commands.CreateRental;

public class CreateRentalCommandHandler : IRequestHandler<CreateRentalCommand, CreatedRentalDto>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly ICarRepository _carRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IRentalAvailabilityService _rentalAvailabilityService; // Injected

    public CreateRentalCommandHandler(
        IRentalRepository rentalRepository,
        ICarRepository carRepository,
        ICustomerRepository customerRepository,
        IRentalAvailabilityService rentalAvailabilityService) // Added to constructor
    {
        _rentalRepository = rentalRepository;
        _carRepository = carRepository;
        _customerRepository = customerRepository;
        _rentalAvailabilityService = rentalAvailabilityService; // Initialized
    }

    public async Task<CreatedRentalDto> Handle(CreateRentalCommand request, CancellationToken cancellationToken)
    {
        // Input validation (dates) is handled by CreateRentalCommandValidator

        // 1. Check if Customer exists
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, asNoTracking: true, cancellationToken);
        if (customer == null)
        {
            throw new NotFoundException($"Customer with ID {request.CustomerId} not found.");
        }

        // 2. Check if Car exists and get its details (including DailyRate)
        // We need to fetch the Car entity with tracking to ensure its properties are available for Rental.Create
        var car = await _carRepository.GetByIdAsync(request.CarId, asNoTracking: false, cancellationToken);
        if (car == null)
        {
            throw new NotFoundException($"Car with ID {request.CarId} not found.");
        }

        // 3. Check car availability using the Domain Service
        var isCarAvailable = await _rentalAvailabilityService.IsCarAvailableAsync(request.CarId, request.StartDate, request.EndDate, null, cancellationToken);
        if (!isCarAvailable)
        {
            throw new BusinessException("CarAvailability", $"Car {car.Model} is not available for the selected period.");
        }

        // 4. Create Rental entity using the factory method
        var rental = Rental.Create(
            Guid.NewGuid(),
            customer, // Pass the Customer entity
            car,      // Pass the Car entity
            request.StartDate,
            request.EndDate,
            car.DailyRate // Use the DailyRate from the fetched Car
        );

        // 5. Add rental to repository
        await _rentalRepository.AddAsync(rental, true, cancellationToken);

        // 6. Map to DTO and return
        return new CreatedRentalDto
        {
            Id = rental.Id,
            CustomerId = rental.CustomerId,
            CarId = rental.CarId,
            StartDate = rental.RentalPeriod.StartDate,
            EndDate = rental.RentalPeriod.EndDate,
            Status = rental.Status.ToString()
        };
    }
}