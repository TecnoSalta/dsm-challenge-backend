using MediatR;
using Microsoft.EntityFrameworkCore; // Added for Include
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Application.Exceptions;
using PWC.Challenge.Common.Exceptions;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Domain.Events.Rentals;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PWC.Challenge.Application.Features.Rentals.Commands.CompleteRental;

public class CompleteRentalCommandHandler : IRequestHandler<CompleteRentalCommand, CompletedRentalDto>
{
    private readonly IBaseRepository<Rental> _rentalRepo;
    private readonly IBaseRepository<Car> _carRepo;
    private readonly IMediator _mediator; // Keep IMediator for publishing domain events if no behavior is in place

    public CompleteRentalCommandHandler(
        IBaseRepository<Rental> rentalRepo,
        IBaseRepository<Car> carRepo,
        IMediator mediator)
    {
        _rentalRepo = rentalRepo;
        _carRepo = carRepo;
        _mediator = mediator;
    }

    public async Task<CompletedRentalDto> Handle(CompleteRentalCommand request, CancellationToken cancellationToken)
    {
        // The check for RouteId vs BodyRentalId is handled by CompleteRentalCommandValidator

        var rental = await _rentalRepo.Query()
            .Include(r => r.Car)
            .FirstOrDefaultAsync(r => r.Id == request.RouteId, cancellationToken); // Use RouteId from command

        if (rental is null)
            throw new NotFoundException(nameof(Rental), request.RouteId);

        if (rental.Status != RentalStatus.Active)
            throw new BusinessException("RentalStatus", "Rental must be Active to be completed."); // Corrected BusinessException

        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

        // 1) Marcar como completado
        rental.Complete(today); // Use today's date for actualReturnDate

        await _rentalRepo.UpdateAsync(rental, true, cancellationToken);
        await _carRepo.UpdateAsync(rental.Car!, true, cancellationToken);

        // Publish domain events if a behavior is not automatically doing it
        // The domain event is added to the aggregate root, and will be published by a MediatR behavior or dispatcher.
        // If a behavior is not in place, this line should be:
        await _mediator.Publish(new RentalCompletedDomainEvent(rental.Id, rental.Car.Id, rental.CustomerId, today), cancellationToken);

        return new CompletedRentalDto(request.RouteId);
    }
}