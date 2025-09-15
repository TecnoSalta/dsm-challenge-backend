using MediatR;
using PWC.Challenge.Domain.Interfaces;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Domain.ValueObjects;
using System;
using System.Threading;
using System.Threading.Tasks;
using PWC.Challenge.Application.Common.Exceptions; // Assuming this path for BusinessException

namespace PWC.Challenge.Application.Features.Rentals.Commands.CreateRental;

public class CreateRentalCommandHandler : IRequestHandler<CreateRentalCommand, CreatedRentalDto>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly ICarRepository _carRepository;
    private readonly ICustomerRepository _customerRepository;

    public CreateRentalCommandHandler(
        IRentalRepository rentalRepository,
        ICarRepository carRepository,
        ICustomerRepository customerRepository)
    {
        _rentalRepository = rentalRepository;
        _carRepository = carRepository;
        _customerRepository = customerRepository;
    }

    public async Task<CreatedRentalDto> Handle(CreateRentalCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate dates
        if (request.StartDate >= request.EndDate)
        {
            throw new BusinessException("Start date must be before end date.");
        }

        if (request.StartDate < DateOnly.FromDateTime(DateTime.Today))
        {
            throw new BusinessException("Start date cannot be in the past.");
        }

        // 2. Check if Customer exists
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, asNoTracking: true, cancellationToken);
        if (customer == null)
        {
            throw new NotFoundException($"Customer with ID {request.CustomerId} not found.");
        }

        // 3. Check if Car exists
        var car = await _carRepository.GetByIdAsync(request.CarId, asNoTracking: true, cancellationToken);
        if (car == null)
        {
            throw new NotFoundException($"Car with ID {request.CarId} not found.");
        }

        // 4. Check car availability
        var isCarAvailable = await _carRepository.IsCarAvailableAsync(request.CarId, request.StartDate, request.EndDate, null);
        if (!isCarAvailable)
        {
            throw new BusinessException($"Car {car.Model} is not available for the selected period.");
        }

        // 5. Create Rental entity
        var rental = new Rental
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            CarId = request.CarId,
            RentalPeriod = new RentalPeriod(request.StartDate, request.EndDate),
            Status = RentalStatus.Pending // Initial status
        };

        // 6. Add rental to repository
        await _rentalRepository.AddAsync(rental, true, cancellationToken);

        // 7. Map to DTO and return
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