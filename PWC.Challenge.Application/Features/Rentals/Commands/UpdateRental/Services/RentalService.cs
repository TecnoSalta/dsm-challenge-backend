using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Application.Exceptions;
using PWC.Challenge.Common.Exceptions;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;

namespace PWC.Challenge.Application.Features.Rentals.Commands.UpdateRental.Services;

public class RentalService : IRentalService
{
    private readonly IBaseRepository<Rental> _rentalRepo;
    private readonly IBaseRepository<Car> _carRepo;

    public RentalService(IBaseRepository<Rental> rentalRepo, IBaseRepository<Car> carRepo)
    {
        _rentalRepo = rentalRepo;
        _carRepo = carRepo;
    }

    public async Task<CancelledRentalDto> CancelRentalAsync(Guid rentalId, CancellationToken ct)
    {
        var rental = await _rentalRepo.QueryTracking()
                                      .FirstOrDefaultAsync(r => r.Id == rentalId, ct)
                   ?? throw new NotFoundException("Rental not found.");

        if (rental.Status != RentalStatus.Active)
            throw new BusinessException("Only active rentals can be cancelled.", string.Empty);

        rental.Cancel();   // tu método de dominio que pone Status = Cancelled y fecha

        await _rentalRepo.UpdateAsync(rental, true, ct); // true -> SaveChanges
        return new CancelledRentalDto(rental.Id, rental.Status.ToString());
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

        var startDate = newStart ?? rental.StartDate;
        var endDate = newEnd ?? rental.EndDate;

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
                          r.StartDate < endDate &&
                          r.EndDate > startDate, ct);

        if (occupied)
            throw new BusinessException("Car is not available in the requested interval.", string.Empty);

        // Usamos el método de dominio para mantener lógica encapsulada
        rental.UpdateRental(startDate, endDate, newCar);

        await _rentalRepo.UpdateAsync(rental, true, ct);

        // Si el coche cambió, actualizamos el estado del nuevo coche
        if (newCar != null)
        {
            newCar.MarkAsRented();
            await _carRepo.UpdateAsync(newCar, false, ct);
        }

        // Creamos el DTO usando el constructor del record
        var dto = new UpdatedRentalDto(
            rental.Id,
            rental.CarId,
            rental.StartDate,
            rental.EndDate,
            "Rental updated successfully."
        );

        return dto;
    }
}