﻿using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Application.Exceptions;
using PWC.Challenge.Common.Exceptions;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;
using MediatR;

namespace PWC.Challenge.Application.Features.Rentals.Commands.UpdateRental.Services;

public class RentalService(IBaseRepository<Rental> rentalRepo, IBaseRepository<Car> carRepo, IMediator mediator) : IRentalService
{
    private readonly IBaseRepository<Rental> _rentalRepo = rentalRepo;
    private readonly IBaseRepository<Car> _carRepo = carRepo;
    private readonly IMediator _mediator = mediator;

    public async Task<CancelledRentalDto> CancelRentalAsync(Guid rentalId, CancellationToken ct)
    {
        var rental = await _rentalRepo.QueryTracking()
                                      .FirstOrDefaultAsync(r => r.Id == rentalId, ct)
                   ?? throw new NotFoundException("Rental not found.");

        // Publica el evento directamente desde la entidad
        await rental.CancelAsync(_mediator);

        await _rentalRepo.UpdateAsync(rental, true, ct);
        return new CancelledRentalDto(rental.Id, rental.Status.ToString());
    }

    public async Task<CompletedRentalDto> CompleteRentalAsync(Guid rentalId, CancellationToken ct)
    {
        var rental = await _rentalRepo.QueryTracking()
                                      .Include(r => r.Car)
                                      .FirstOrDefaultAsync(r => r.Id == rentalId, ct)
                     ?? throw new NotFoundException("Rental not found.");
        if (rental.Status != RentalStatus.Active)
            throw new BusinessException("Only active rentals can be modified.", string.Empty);

        Car car = await _carRepo.GetByIdAsync(rental.CarId, false, ct)
                     ?? throw new NotFoundException(nameof(Car), rental.CarId);
        
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        rental.Complete(today);

        await _rentalRepo.UpdateAsync(rental, true, ct);


        if (car != null)
        {
            car.MarkAsAvailable();
            await _carRepo.UpdateAsync(car, false, ct);
        }

        var dto = new CompletedRentalDto(
           rental.Id
       );

        return dto;
    }

    public async Task<UpdatedRentalDto> UpdateRentalAsync(
        Guid rentalId,
        DateOnly? newStart,
        DateOnly? newEnd,
        Guid? newCarId,
        CancellationToken ct)
    {
        var rental = await _rentalRepo.QueryTracking()
                                      .Include(r => r.Car)
                                      .FirstOrDefaultAsync(r => r.Id == rentalId, ct)
                     ?? throw new NotFoundException("Rental not found.");

        if (rental.Status != RentalStatus.Active)
            throw new BusinessException("Only active rentals can be modified.", string.Empty);

        var startDate = newStart ?? rental.RentalPeriod.StartDate;
        var endDate = newEnd ?? rental.RentalPeriod.EndDate;

        Car? newCar = null;
        if (newCarId.HasValue && newCarId.Value != rental.CarId)
        {
            newCar = await _carRepo.GetByIdAsync(newCarId.Value, false, ct)
                     ?? throw new NotFoundException("Car not found.");
        }

        // Validamos disponibilidad antes de aplicar cambios
        var carIdToCheck = newCar?.Id ?? rental.CarId;
        bool occupied = await _rentalRepo.Query()
            .AnyAsync(r => r.Id != rental.Id &&
                          r.CarId == carIdToCheck &&
                          r.Status == RentalStatus.Active &&                          
                          r.RentalPeriod.StartDate < endDate &&
                          r.RentalPeriod.EndDate > startDate, ct);

        if (occupied)
            throw new BusinessException("Car is not available in the requested interval.", string.Empty);

        // Aplicar cambios usando los métodos de dominio
        if (newCar != null)
        {
            // Asumimos que si se cambia el coche, la tarifa también puede cambiar.
            // Aquí usamos la tarifa del coche nuevo como ejemplo.
            rental.ChangeCar(newCar, newCar.DailyRate);
        }
        
        if (newStart.HasValue || newEnd.HasValue)
        {
            rental.UpdateRentalDates(startDate, endDate);
        }

        await _rentalRepo.UpdateAsync(rental, true, ct);

        // Si el coche cambió, actualizamos el estado del coche anterior y el nuevo
        if (newCar != null)
        {
            var originalCar = await _carRepo.GetByIdAsync(rental.CarId, false, ct);
            originalCar?.MarkAsAvailable(); // El coche original vuelve a estar disponible
            if (originalCar != null) await _carRepo.UpdateAsync(originalCar, false, ct);

            newCar.MarkAsRented();
            await _carRepo.UpdateAsync(newCar, false, ct);
        }

        var dto = new UpdatedRentalDto(
            rental.Id,
            rental.CarId,
            rental.RentalPeriod.StartDate,
            rental.RentalPeriod.EndDate,
            "Rental updated successfully."
        );

        return dto;
    }
}
