using MediatR;
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Domain.Interfaces;
using PWC.Challenge.Application.Exceptions;
using PWC.Challenge.Common.Exceptions;

namespace PWC.Challenge.Application.Features.Rentals.Queries.GetRentalById;

public class GetRentalByIdQueryHandler : IRequestHandler<GetRentalByIdQuery, RentalDto>
{
    private readonly IRentalRepository _rentalRepository;

    public GetRentalByIdQueryHandler(IRentalRepository rentalRepository)
    {
        _rentalRepository = rentalRepository;
    }

    public async Task<RentalDto> Handle(GetRentalByIdQuery request, CancellationToken cancellationToken)
    {
        var rental = await _rentalRepository.GetByIdAsync(request.Id, false,cancellationToken);

        if (rental == null)
        {
            throw new NotFoundException($"Rental with ID {request.Id} not found.");
        }

        return new RentalDto
        {
            Id = rental.Id,
            CustomerId = rental.CustomerId,
            CarId = rental.CarId,
            StartDate = rental.RentalPeriod.StartDate,
            EndDate = rental.RentalPeriod.EndDate,
            Status = rental.Status,
            DailyRate = rental.DailyRate,
            TotalCost = rental.TotalCost,
            ActualReturnDate = rental.ActualReturnDate
        };
    }
}
