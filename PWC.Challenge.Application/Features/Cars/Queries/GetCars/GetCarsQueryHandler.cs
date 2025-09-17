using MediatR;
using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWC.Challenge.Application.Features.Cars.Queries.GetCars;

public class GetCarsQueryHandler : IRequestHandler<GetCarsQuery, IReadOnlyList<CarDto>>
{
    private readonly ICarRepository _carRepository;

    public GetCarsQueryHandler(ICarRepository carRepository)
    {
        _carRepository = carRepository;
    }

    public async Task<IReadOnlyList<CarDto>> Handle(GetCarsQuery request, CancellationToken cancellationToken)
    {
        var cars = await _carRepository.GetAllWithServicesAsync(asNoTracking: true, cancellationToken: cancellationToken);

        return cars.Select(car => new CarDto
        {
            Id = car.Id,
            Type = car.Type,
            Model = car.Model,
            DailyRate = car.DailyRate,
            LicensePlate = car.LicensePlate, // Added this line
            Services = car.Services.Select(s => new ServiceDto
            {
                Date = s.Date,
                DurationDays = s.DurationDays
            }).ToList()
        }).ToList();
    }
}
