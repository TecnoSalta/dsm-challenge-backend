using MediatR;
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWC.Challenge.Application.Features.Rentals.Queries.GetRentals;

public class GetRentalsQueryHandler : IRequestHandler<GetRentalsQuery, IReadOnlyList<RentalDto>>
{
    private readonly IRentalRepository _rentalRepository;

    public GetRentalsQueryHandler(IRentalRepository rentalRepository)
    {
        _rentalRepository = rentalRepository;
    }

    public async Task<IReadOnlyList<RentalDto>> Handle(GetRentalsQuery request, CancellationToken cancellationToken)
    {
        var rentals = await _rentalRepository.GetAllAsync(cancellationToken);

        return rentals.Select(rental => new RentalDto
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
        }).ToList();
    }
}
