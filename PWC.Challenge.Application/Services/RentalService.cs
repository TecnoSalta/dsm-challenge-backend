using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Exceptions;
using PWC.Challenge.Domain.Interfaces;
using PWC.Challenge.Domain.ValueObjects; // For RentalPeriod
using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Application.Exceptions; // For Include extension method

namespace PWC.Challenge.Application.Services;

public class RentalService : IRentalService
{
    private readonly ICarRepository _carRepository;
    private readonly IBaseRepository<Customer> _customerRepository;
    private readonly IBaseRepository<Rental> _rentalRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RentalService(ICarRepository carRepository, IBaseRepository<Customer> customerRepository, IBaseRepository<Rental> rentalRepository, IUnitOfWork unitOfWork)
    {
        _carRepository = carRepository;
        _customerRepository = customerRepository;
        _rentalRepository = rentalRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Rental> RegisterRentalAsync(CreateRentalRequestDto request)
    {
        // 1. Get Car by Id with its rentals and services for availability check
        var car = await _carRepository.GetSingleAsync(
            c => c.Id == request.CarId,
            includes: query => query.Include(c => c.Rentals).Include(c => c.Services),
            asNoTracking: false
        );
        if (car == null)
        {
            throw new EntityNotFoundException<Guid>(nameof(Car), request.CarId);
        }

        // 2. Get Customer by Id
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, asNoTracking: false);
        if (customer == null)
        {
            throw new EntityNotFoundException<Guid>(nameof(Customer), request.CustomerId);
        }

        // 3. Validate Car Availability (already done by Car.AddRental, but good to have an explicit check here too)
        if (!car.IsAvailableForPeriod(request.StartDate, request.EndDate))
        {
            throw new CarNotAvailableException(car.Id);
        }

        // 4. Create Rental Entity
        var rentalPeriod = new RentalPeriod(request.StartDate, request.EndDate);
        var rental = Rental.Create(
            Guid.NewGuid(), // New Id for the rental
            customer,
            car,
            request.StartDate,
            request.EndDate,
            car.DailyRate // Use car's daily rate
        );

        // 5. Invoke Car.AddRental() to protect invariants and add to car's collection
        car.AddRental(rental); // This also adds a domain event to the car

        // 6. Add rental to customer's collection (optional, but good for consistency)
        customer.AddRental(rental);

        // 7. Add the new Rental entity to its repository
        await _rentalRepository.AddAsync(rental);

        await _unitOfWork.SaveChangesAsync(); // Save changes to Car and new Rental

        return rental;
    }
}