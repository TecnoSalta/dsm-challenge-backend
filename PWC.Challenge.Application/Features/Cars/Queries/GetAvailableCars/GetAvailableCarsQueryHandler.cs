using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Common.CQRS;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Domain.Interfaces;
using PWC.Challenge.Domain.Services;

namespace PWC.Challenge.Application.Features.Cars.Queries.GetAvailableCars
{
    public class GetAvailableCarsQueryHandler
        : IQueryHandler<GetAvailableCarsQuery, IReadOnlyList<AvailableCarDto>>
    {
        private readonly ICarRepository _carRepository;
        private readonly IRentalRepository _rentalRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IAvailabilityService _availabilityService;

        public GetAvailableCarsQueryHandler(
            ICarRepository carRepository,
            IRentalRepository rentalRepository,
            IServiceRepository serviceRepository,
            IAvailabilityService availabilityService)
        {
            _carRepository = carRepository;
            _rentalRepository = rentalRepository;
            _serviceRepository = serviceRepository;
            _availabilityService = availabilityService;
        }

        public async Task<IReadOnlyList<AvailableCarDto>> Handle(
            GetAvailableCarsQuery request,
            CancellationToken ct)
        {
            var (startDate, endDate, carType, model) = request.Filter;

            // Validar fechas
            ValidateDates(startDate, endDate);

            // Obtener todos los autos activos
            var allCars = await _carRepository.GetAllAsync();
            var availableCars = new List<Car>();

            foreach (var car in allCars)
            {
                // Aplicar filtros primero (performance)
                if (!PassesFilters(car, carType, model))
                    continue;

                // Verificar disponibilidad completa
                var isAvailable = await _availabilityService.IsCarAvailableAsync(
                    car.Id, startDate, endDate);

                if (isAvailable)
                {
                    availableCars.Add(car);
                }
            }

            return availableCars
                .Select(c => new AvailableCarDto(c.Id, c.Type, c.Model, c.DailyRate))
                .ToList();
        }

        private void ValidateDates(DateOnly startDate, DateOnly endDate)
        {
            if (startDate >= endDate)
                throw new ArgumentException("End date must be after start date");

            if (endDate.DayNumber - startDate.DayNumber < 1)
                throw new ArgumentException("Rental period must be at least 1 day");

            if (startDate < DateOnly.FromDateTime(DateTime.UtcNow))
                throw new ArgumentException("Start date cannot be in the past");
        }

        private bool PassesFilters(Car car, string? carType, string? model)
        {
            if (!string.IsNullOrEmpty(carType) && !car.Type.Equals(carType, StringComparison.OrdinalIgnoreCase))
                return false;

            if (!string.IsNullOrEmpty(model) && !car.Model.Contains(model, StringComparison.OrdinalIgnoreCase))
                return false;

            return car.Status == CarStatus.Available;
        }
    }
}