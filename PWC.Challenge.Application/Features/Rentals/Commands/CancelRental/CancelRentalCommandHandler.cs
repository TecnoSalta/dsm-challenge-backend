using MediatR;
using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Common.Exceptions;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Events.Rentals;

namespace PWC.Challenge.Application.Features.Rentals.Commands.CancelRental;

public class CancelRentalCommandHandler : IRequestHandler<CancelRentalCommand, CancelledRentalDto>
{
    private readonly IBaseRepository<Rental> _rentalRepo;
    private readonly IMediator _mediator;

    public CancelRentalCommandHandler(
        IBaseRepository<Rental> rentalRepo,
        IMediator mediator)
    {
        _rentalRepo = rentalRepo;
        _mediator = mediator;
    }

    public async Task<CancelledRentalDto> Handle(CancelRentalCommand request, CancellationToken cancellationToken)
    {
        var rental = await _rentalRepo.QueryTracking()
                                      .FirstOrDefaultAsync(r => r.Id == request.RentalId, cancellationToken)
                     ?? throw new NotFoundException(nameof(Rental), request.RentalId);

        // Publica el evento directamente desde la entidad
        rental.Cancel(); // Call the domain method

        await _rentalRepo.UpdateAsync(rental, true, cancellationToken);

        // Publish domain events if a behavior is not automatically doing it
     
        await _mediator.Publish(new RentalCancelledDomainEvent(rental.Id, rental.CarId, rental.CustomerId), cancellationToken);

        return new CancelledRentalDto(rental.Id, rental.Status.ToString());
    }
}