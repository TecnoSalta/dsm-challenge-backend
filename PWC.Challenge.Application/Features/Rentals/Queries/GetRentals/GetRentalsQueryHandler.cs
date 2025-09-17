using MediatR;
using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Domain.Interfaces;

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
        // Use the new method to get rentals with eager-loaded customers
        var rentals = await _rentalRepository.GetAllWithCustomersAsync(asNoTracking: true, cancellationToken: cancellationToken);

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
            ActualReturnDate = rental.ActualReturnDate,
            Customer = rental.Customer != null ? new CustomerDto
            {
                Id = rental.Customer.Id,
                Dni = rental.Customer.Dni,
                FullName = rental.Customer.FullName,
                Address = rental.Customer.Address
            } : null
        }).ToList();
    }
}
