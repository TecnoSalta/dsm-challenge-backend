using MediatR;
using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Application.Exceptions;
// Removed: using PWC.Challenge.Common.Exceptions;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Domain.Services; // For IRentalAvailabilityService
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PWC.Challenge.Application.Features.Rentals.Commands.UpdateRental;

public class UpdateRentalCommandHandler
    : IRequestHandler<UpdateRentalCommand, UpdatedRentalDto>
{
    private readonly IBaseRepository<Rental> _rentalRepo;
    private readonly IBaseRepository<Car> _carRepo;
    private readonly IMediator _mediator;
    private readonly IRentalAvailabilityService _rentalAvailabilityService; // Changed to IRentalAvailabilityService

    public UpdateRentalCommandHandler(
        IBaseRepository<Rental> rentalRepo,
        IBaseRepository<Car> carRepo,
        IMediator mediator,
        IRentalAvailabilityService rentalAvailabilityService) // Changed to IRentalAvailabilityService
    {
        _rentalRepo = rentalRepo;
        _carRepo = carRepo;
        _mediator = mediator;
        _rentalAvailabilityService = rentalAvailabilityService; // Initialized
    }

    /// <summary>
    /// Dada una reserva existente, actualiza sus fechas y/o coche.
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<UpdatedRentalDto> Handle(
        UpdateRentalCommand cmd,
        CancellationToken cancellationToken)
    {
        var rental = await _rentalRepo.QueryTracking()
                                      .Include(r => r.Car)
                                      .FirstOrDefaultAsync(r => r.Id == cmd.RentalId, cancellationToken)
                     ?? throw new NotFoundException(nameof(Rental), cmd.RentalId);

        if (rental.Status != RentalStatus.Active)
            throw new BusinessException("RentalStatus", "Only active rentals can be modified.");

        var startDate = cmd.Payload.NewStartDate ?? rental.RentalPeriod.StartDate;
        var endDate = cmd.Payload.NewEndDate ?? rental.RentalPeriod.EndDate;

        var originalCar = rental.Car; // Keep a reference to the original car

        Car? newCar = null;
        if (cmd.Payload.NewCarId.HasValue && cmd.Payload.NewCarId.Value != rental.CarId)
        {
            newCar = await _carRepo.GetByIdAsync(cmd.Payload.NewCarId.Value, false, cancellationToken)
                     ?? throw new NotFoundException("Car not found.");
        }

        // Validamos disponibilidad antes de aplicar cambios
        var carIdToCheck = newCar?.Id ?? rental.CarId;
        var isAvailable = await _rentalAvailabilityService.IsCarAvailableAsync(carIdToCheck, startDate, endDate, rental.Id, cancellationToken); // Changed method call

        if (!isAvailable)
            throw new BusinessException("CarAvailability", "Car is not available in the requested interval.");

        // Aplicar cambios usando los métodos de dominio
        if (newCar != null)
        {
            // Asumimos que si se cambia el coche, la tarifa también puede cambiar.
            // Aquí usamos la tarifa del coche nuevo como ejemplo.
            rental.ChangeCar(newCar, newCar.DailyRate);
        }
        
        if (cmd.Payload.NewStartDate.HasValue || cmd.Payload.NewEndDate.HasValue)
        {
            rental.UpdateRentalDates(startDate, endDate);
        }

        await _rentalRepo.UpdateAsync(rental, true, cancellationToken);

        // Si el coche cambió, actualizamos el estado del coche anterior y el nuevo
        if (newCar != null)
        {
            originalCar.MarkAsAvailable();
            await _carRepo.UpdateAsync(originalCar, false, cancellationToken);

            newCar.MarkAsRented();
            await _carRepo.UpdateAsync(newCar, false, cancellationToken);
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