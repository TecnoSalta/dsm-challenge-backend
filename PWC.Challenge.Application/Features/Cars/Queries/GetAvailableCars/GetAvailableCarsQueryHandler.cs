using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Common.CQRS;
using PWC.Challenge.Domain.Interfaces;

namespace PWC.Challenge.Application.Features.Cars.Queries.GetAvailableCars
{
    public class GetAvailableCarsQueryHandler
        : IQueryHandler<GetAvailableCarsQuery, IReadOnlyList<AvailableCarDto>>
    {
        private readonly ICarRepository _carRepository;

        public GetAvailableCarsQueryHandler(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        public async Task<IReadOnlyList<AvailableCarDto>> Handle(
            GetAvailableCarsQuery request,
            CancellationToken ct)
        {
            var (startDate, endDate, carType, model) = request.Filter;

            // Validar fechas
            if (startDate >= endDate)
                throw new ArgumentException("End date must be after start date");

            if (endDate.DayNumber - startDate.DayNumber < 1)
                throw new ArgumentException("Rental period must be at least 1 day");

            // Obtener autos disponibles usando el repositorio especializado
            var availableCars = await _carRepository.GetAvailableCarsAsync(
                startDate, endDate, carType, model);

            return availableCars
                .Select(c => new AvailableCarDto(c.Id, c.Type, c.Model, c.DailyRate))
                .ToList();
        }
    }
}