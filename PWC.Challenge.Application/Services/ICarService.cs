using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Application.Services;

public interface ICarService
{
    Task<IEnumerable<AvailableCarDto>> GetAvailableCarsAsync(DateOnly startDate, DateOnly endDate, string? carType, string? carModel);
    Task<CarAvailabilityDto> GetCarAvailabilityAsync(Guid carId, DateOnly startDate, DateOnly endDate);
}