using Microsoft.Extensions.Logging;
using Moq;
using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Application.Features.Cars.Queries.GetAvailableCars;
using PWC.Challenge.Application.Interfaces;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Interfaces;
using PWC.Challenge.Domain.Services;

namespace PWC.Challenge.UnitTests.Features.Availability.Queries
{
    public class GetAvailableCarsQueryHandlerTests
    {
        private readonly Mock<ICarRepository> _carRepositoryMock;
        private readonly Mock<IAvailabilityService> _availabilityServiceMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<ILogger<GetAvailableCarsQueryHandler>> _loggerMock;
        private readonly GetAvailableCarsQueryHandler _handler;

        public GetAvailableCarsQueryHandlerTests()
        {
            _carRepositoryMock = new Mock<ICarRepository>();
            _availabilityServiceMock = new Mock<IAvailabilityService>();
            _cacheServiceMock = new Mock<ICacheService>();
            _loggerMock = new Mock<ILogger<GetAvailableCarsQueryHandler>>();

            _handler = new GetAvailableCarsQueryHandler(
                _carRepositoryMock.Object,
                _availabilityServiceMock.Object,
                _cacheServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnAvailableCars_WhenNoConflicts()
        {
            // Arrange
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3));
            var query = new GetAvailableCarsQuery(new AvailabilityQueryDto(startDate, endDate));

            var cars = new List<Car>
            {
                new Car(Guid.NewGuid(), "Sedan", "Toyota Camry", 50m),
                new Car(Guid.NewGuid(), "SUV", "Honda CR-V", 70m)
            };

            var expectedResult = cars
                .Select(c => new AvailableCarDto(c.Id, c.Type, c.Model, c.DailyRate))
                .ToList();

            _cacheServiceMock
                .Setup(c => c.GetAsync<IReadOnlyList<AvailableCarDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IReadOnlyList<AvailableCarDto>)null); // Cache miss

            _carRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cars);

            _availabilityServiceMock.Setup(service =>
                service.IsCarAvailableAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<Guid?>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _availabilityServiceMock.Verify(service =>
                service.IsCarAvailableAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<Guid?>()),
                Times.Exactly(2));

            _cacheServiceMock.Verify(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<IReadOnlyList<AvailableCarDto>>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnCachedResult_WhenCacheHit()
        {
            // Arrange
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3));
            var query = new GetAvailableCarsQuery(new AvailabilityQueryDto(startDate, endDate));

            var cachedCars = new List<AvailableCarDto>
            {
                new AvailableCarDto(Guid.NewGuid(), "Sedan", "Toyota Camry", 50m),
                new AvailableCarDto(Guid.NewGuid(), "SUV", "Honda CR-V", 70m)
            };

            _cacheServiceMock
                .Setup(c => c.GetAsync<IReadOnlyList<AvailableCarDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedCars); // Cache hit

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(cachedCars, result);

            // Verify that repository and availability service were not called
            _carRepositoryMock.Verify(repo => repo.GetAllAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
            _availabilityServiceMock.Verify(service =>
                service.IsCarAvailableAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<Guid?>()),
                Times.Never);

            _cacheServiceMock.Verify(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<IReadOnlyList<AvailableCarDto>>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldFilterByCarType_WhenSpecified()
        {
            // Arrange
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3));
            var query = new GetAvailableCarsQuery(new AvailabilityQueryDto(startDate, endDate, "SUV"));

            var cars = new List<Car>
            {
                new Car(Guid.NewGuid(), "Sedan", "Toyota Camry", 50m),
                new Car(Guid.NewGuid(), "SUV", "Honda CR-V", 70m),
                new Car(Guid.NewGuid(), "SUV", "Ford Explorer", 80m)
            };

            _cacheServiceMock
                .Setup(c => c.GetAsync<IReadOnlyList<AvailableCarDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IReadOnlyList<AvailableCarDto>)null);

            _carRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cars);

            _availabilityServiceMock.Setup(service =>
                service.IsCarAvailableAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<Guid?>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, car => Assert.Equal("SUV", car.Type));
        }

        [Fact]
        public async Task Handle_ShouldFilterByModel_WhenSpecified()
        {
            // Arrange
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3));
            var query = new GetAvailableCarsQuery(new AvailabilityQueryDto(startDate, endDate, null, "Camry"));

            var cars = new List<Car>
            {
                new Car(Guid.NewGuid(), "Sedan", "Toyota Camry", 50m),
                new Car(Guid.NewGuid(), "Sedan", "Honda Accord", 55m),
                new Car(Guid.NewGuid(), "SUV", "Honda CR-V", 70m)
            };

            _cacheServiceMock
                .Setup(c => c.GetAsync<IReadOnlyList<AvailableCarDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IReadOnlyList<AvailableCarDto>)null);

            _carRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cars);

            _availabilityServiceMock.Setup(service =>
                service.IsCarAvailableAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<Guid?>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Toyota Camry", result.First().Model);
        }

        [Fact]
        public async Task Handle_ShouldExcludeUnavailableCars_WhenServiceScheduled()
        {
            // Arrange
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3));
            var query = new GetAvailableCarsQuery(new AvailabilityQueryDto(startDate, endDate));

            var cars = new List<Car>
            {
                new Car(Guid.NewGuid(), "Sedan", "Toyota Camry", 50m),
                new Car(Guid.NewGuid(), "SUV", "Honda CR-V", 70m)
            };

            _cacheServiceMock
                .Setup(c => c.GetAsync<IReadOnlyList<AvailableCarDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IReadOnlyList<AvailableCarDto>)null);

            _carRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cars);

            _availabilityServiceMock.SetupSequence(service =>
                service.IsCarAvailableAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<Guid?>()))
                .ReturnsAsync(true)   // First car available
                .ReturnsAsync(false); // Second car unavailable due to service

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Toyota Camry", result.First().Model);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenStartDateInPast()
        {
            // Arrange
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)); // Past date
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3));
            var query = new GetAvailableCarsQuery(new AvailabilityQueryDto(startDate, endDate));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldUseCorrectCacheKey_ForDifferentParameters()
        {
            // Arrange
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3));
            var carType = "SUV";
            var model = "CR-V";

            var query1 = new GetAvailableCarsQuery(new AvailabilityQueryDto(startDate, endDate));
            var query2 = new GetAvailableCarsQuery(new AvailabilityQueryDto(startDate, endDate, carType));
            var query3 = new GetAvailableCarsQuery(new AvailabilityQueryDto(startDate, endDate, null, model));
            var query4 = new GetAvailableCarsQuery(new AvailabilityQueryDto(startDate, endDate, carType, model));

            string capturedKey = null;
            _cacheServiceMock
                .Setup(c => c.GetAsync<IReadOnlyList<AvailableCarDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string, CancellationToken>((key, ct) => capturedKey = key)
                .ReturnsAsync((IReadOnlyList<AvailableCarDto>)null);

            _carRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Car> { new Car(Guid.NewGuid(), "SUV", "CR-V", 70m) });

            _availabilityServiceMock.Setup(service =>
                service.IsCarAvailableAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<Guid?>()))
                .ReturnsAsync(true);

            // Act & Assert for each query
            await _handler.Handle(query1, CancellationToken.None);
            Assert.Contains("available_cars:", capturedKey);
            Assert.Contains(startDate.ToString("yyyyMMdd"), capturedKey);
            Assert.Contains(endDate.ToString("yyyyMMdd"), capturedKey);
            Assert.Contains("all:all", capturedKey);

            await _handler.Handle(query2, CancellationToken.None);
            Assert.Contains($"available_cars:{startDate:yyyyMMdd}:{endDate:yyyyMMdd}:SUV:all", capturedKey);

            await _handler.Handle(query3, CancellationToken.None);
            Assert.Contains($"available_cars:{startDate:yyyyMMdd}:{endDate:yyyyMMdd}:all:CR-V", capturedKey);

            await _handler.Handle(query4, CancellationToken.None);
            Assert.Contains($"available_cars:{startDate:yyyyMMdd}:{endDate:yyyyMMdd}:SUV:CR-V", capturedKey);
        }

        [Fact]
        public async Task Handle_ShouldHandleCacheExceptions_Gracefully()
        {
            // Arrange
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3));
            var query = new GetAvailableCarsQuery(new AvailabilityQueryDto(startDate, endDate));

            var cars = new List<Car>
    {
        new Car(Guid.NewGuid(), "Sedan", "Toyota Camry", 50m)
    };

            // Simular excepción en el cache para la operación GET
            _cacheServiceMock
                .Setup(c => c.GetAsync<IReadOnlyList<AvailableCarDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Cache error"));

            _carRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cars);

            _availabilityServiceMock.Setup(service =>
                service.IsCarAvailableAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<Guid?>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert - debería continuar funcionando a pesar del error de cache
            Assert.NotNull(result);
            Assert.Single(result);

            // Verify que se intentó guardar en cache a pesar del error inicial
            _cacheServiceMock.Verify(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<IReadOnlyList<AvailableCarDto>>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
                Times.Once);

            // Verify que se llamó al repositorio como fallback
            _carRepositoryMock.Verify(repo => repo.GetAllAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldExcludeCarsInMaintenance()
        {
            // Arrange
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3));
            var query = new GetAvailableCarsQuery(new AvailabilityQueryDto(startDate, endDate));

            var availableCar = new Car(Guid.NewGuid(), "Sedan", "Toyota Camry", 50m);
            var maintenanceCar = new Car(Guid.NewGuid(), "SUV", "Honda CR-V", 70m);
            maintenanceCar.MarkAsInMaintenance(); // Mark as unavailable

            var cars = new List<Car> { availableCar, maintenanceCar };

            _cacheServiceMock
                .Setup(c => c.GetAsync<IReadOnlyList<AvailableCarDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IReadOnlyList<AvailableCarDto>)null);

            _carRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cars);

            _availabilityServiceMock.Setup(service =>
                service.IsCarAvailableAsync(availableCar.Id, It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<Guid?>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Toyota Camry", result.First().Model);

            // Verify que el carro en mantenimiento ni siquiera se verificó su disponibilidad
            _availabilityServiceMock.Verify(service =>
                service.IsCarAvailableAsync(maintenanceCar.Id, It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<Guid?>()),
                Times.Never);
        }
    }
}