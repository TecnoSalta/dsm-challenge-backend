using Mapster;
using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Application.Exceptions;
using PWC.Challenge.Common.CQRS;
using PWC.Challenge.Common.Exceptions;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;

namespace PWC.Challenge.Application.Features.Commands.Rentals.UpdateReservation;

public class UpdateReservationCommandHandler
    : ICommandHandler<UpdateReservationCommand, UpdatedReservationDto>
{
    private readonly IBaseRepository<Rental> _rentalRepo;
    private readonly IBaseRepository<Car> _carRepo;

    public UpdateReservationCommandHandler(
        IBaseRepository<Rental> rentalRepo,
        IBaseRepository<Car> carRepo
        )
    {
        _rentalRepo = rentalRepo;
        _carRepo = carRepo;
    }

   

    public async Task<UpdatedReservationDto> Handle(
        UpdateReservationCommand cmd,
        CancellationToken ct)
    {
        var rental = await _rentalRepo.QueryTracking()
                                     .Include(r => r.Car)
                                     .FirstOrDefaultAsync(r => r.Id == cmd.ReservationId, ct)
                   ?? throw new NotFoundException("Rental not found.");

        if (rental.Status != RentalStatus.Active)
            throw new BusinessException("Only active rentals can be modified.","");

        var newStart = cmd.Payload.NewStartDate ?? rental.StartDate;
        var newEnd = cmd.Payload.NewEndDate ?? rental.EndDate;
        var newCarId = cmd.Payload.NewCarId ?? rental.CarId;

        var carChanged = newCarId != rental.CarId;
        var dateChanged = newStart != rental.StartDate || newEnd != rental.EndDate;

        if (carChanged || dateChanged)
        {
            var occupied = await _rentalRepo.Query()
                .AnyAsync(r => r.Id != rental.Id &&
                              r.CarId == newCarId &&
                              r.Status == RentalStatus.Active &&
                              r.StartDate < newEnd &&
                              r.EndDate > newStart, ct);

            if (occupied)
                throw new BusinessException("Car is not available in the requested interval.", "");
        }

        rental.StartDate = newStart;
        rental.EndDate = newEnd;
        await _rentalRepo.UpdateAsync(rental, false, ct);
        var car = await _carRepo.GetByIdAsync(newCarId,false, ct)
                  ?? throw new NotFoundException("Car not found.");
        car.Status = CarStatus.Rented;
        if (carChanged) await _carRepo.UpdateAsync(car);


        return rental.Adapt<UpdatedReservationDto>() with
        {
            Message = "Reservation updated successfully."
        };
    }
}