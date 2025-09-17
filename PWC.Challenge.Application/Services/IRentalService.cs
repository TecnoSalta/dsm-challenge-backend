using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Application.Services;

public interface IRentalService
{
    Task<Rental> RegisterRentalAsync(CreateRentalRequestDto request);
}