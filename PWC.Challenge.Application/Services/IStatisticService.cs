using PWC.Challenge.Application.Dtos;

namespace PWC.Challenge.Application.Services;

public interface IStatisticService
{
    Task<List<CarTypeRentalCountDto>> GetMostRentedCarTypesAsync(DateOnly? startDate = null, DateOnly? endDate = null);
}
