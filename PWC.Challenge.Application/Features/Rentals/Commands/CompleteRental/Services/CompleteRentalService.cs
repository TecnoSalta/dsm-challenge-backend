using MediatR;
using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Common.Exceptions;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Domain.Events.Rentals;

namespace PWC.Challenge.Application.Features.Rentals.Commands.CompleteRental.Services;

public class CompleteRentalService : ICompleteRentalService
{
    private readonly IBaseRepository<Rental> _rentalRepo;
    private readonly IBaseRepository<Car> _carRepo;
    private readonly IMediator _mediator;

    public CompleteRentalService(
        IBaseRepository<Rental> rentalRepo,
        IBaseRepository<Car> carRepo,
        IBaseRepository<Service> serviceRepo,
        IMediator mediator)
    {
        _rentalRepo = rentalRepo;
        _carRepo = carRepo;
        _mediator = mediator;
    }

    public async Task<CompletedRentalDto> CompleteAsync(Guid rentalId, DateOnly? actualReturnDate, CancellationToken cancellationToken = default)
    {
        var rental = await _rentalRepo.Query()
            .Include(r => r.Car)
            .FirstOrDefaultAsync(r => r.Id == rentalId, cancellationToken);

        if (rental is null)
            throw new NotFoundException(nameof(Rental), rentalId);

        if (rental.Status != RentalStatus.Active)
            throw new InvalidOperationException("Rental must be Active to be completed.");
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        // 1) Marcar como completado
        rental.Complete(actualReturnDate ?? today);

        //TODO: Revisar si el coche se devuelve antes o después de la fecha pactada
        // 2) Revisar si el coche necesita service
        /* if (rental.Car is not null &&
             rental.Car.LastServiceDate.AddMonths(2) <= DateTime.UtcNow)
         {
             var service = new Service(
                 Guid.NewGuid(),
                 rental.Car.Id,
                 DateTime.UtcNow,
                 DateTime.UtcNow.AddDays(2)
             );

             await _serviceRepo.AddAsync(service, cancellationToken);

             rental.Car.MarkUnavailable();
         }
         else
         {
             // cooldown de 1 día (mañana no disponible)
             rental.Car?.MarkUnavailable();
         }*/

        await _rentalRepo.UpdateAsync(rental, true, cancellationToken);
        await _carRepo.UpdateAsync(rental.Car!, true, cancellationToken);
        // 3) Emitir evento de dominio
        rental.AddDomainEvent(new RentalCompletedDomainEvent(rental.Id, rental.Car.Id, rental.CustomerId,today));

        await _mediator.Publish(new RentalCompletedDomainEvent(rental.Id, rental.Car.Id,rental.CustomerId, today), cancellationToken);
        return new CompletedRentalDto(rentalId);
        
    }
}
