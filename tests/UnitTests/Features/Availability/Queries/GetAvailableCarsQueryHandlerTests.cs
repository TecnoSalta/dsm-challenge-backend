using Moq;
using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Application.Features.Cars.Queries.GetAvailableCars;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Domain.Interfaces;
using PWC.Challenge.Domain.Services;

namespace PWC.Challenge.UnitTests.Features.Availability.Queries
{
    public class GetAvailableCarsQueryHandlerTests
    {
        private readonly Mock<ICarRepository> _carRepositoryMock;
        private readonly Mock<IRentalRepository> _rentalRepositoryMock;
        private readonly Mock<IServiceRepository> _serviceRepositoryMock;
        private readonly Mock<IAvailabilityService> _availabilityServiceMock;
        private readonly GetAvailableCarsQueryHandler _handler;

        public GetAvailableCarsQueryHandlerTests()
        {
            _carRepositoryMock = new Mock<ICarRepository>();
            _rentalRepositoryMock = new Mock<IRentalRepository>();
            _serviceRepositoryMock = new Mock<IServiceRepository>();
            _availabilityServiceMock = new Mock<IAvailabilityService>();

            _handler = new GetAvailableCarsQueryHandler(
                _carRepositoryMock.Object,
                _rentalRepositoryMock.Object,
                _serviceRepositoryMock.Object,
                _availabilityServiceMock.Object);
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

            _carRepositoryMock
                .Setup(repo => repo.GetAllAsync( It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cars);
            _availabilityServiceMock.Setup(service =>
                service.IsCarAvailableAsync(It.IsAny<Guid>(), startDate, endDate, null))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _availabilityServiceMock.Verify(service =>
                service.IsCarAvailableAsync(It.IsAny<Guid>(), startDate, endDate, null),
                Times.Exactly(2));
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


            _carRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cars);

            _availabilityServiceMock.Setup(service =>
                service.IsCarAvailableAsync(It.IsAny<Guid>(), startDate, endDate, null))
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


            _carRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cars);

            _availabilityServiceMock.Setup(service =>
                service.IsCarAvailableAsync(It.IsAny<Guid>(), startDate, endDate, null))
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


            _carRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cars);

            _availabilityServiceMock.SetupSequence(service =>
                service.IsCarAvailableAsync(It.IsAny<Guid>(), startDate, endDate, null))
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
        public async Task Handle_ShouldThrowException_WhenEndDateBeforeStartDate()
        {
            // Arrange
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3));
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)); // End before start
            var query = new GetAvailableCarsQuery(new AvailabilityQueryDto(startDate, endDate));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenRentalPeriodLessThanOneDay()
        {
            // Arrange
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var endDate = startDate; // Same day
            var query = new GetAvailableCarsQuery(new AvailabilityQueryDto(startDate, endDate));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(query, CancellationToken.None));
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


            _carRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cars);

            _availabilityServiceMock.Setup(service =>
                service.IsCarAvailableAsync(availableCar.Id, startDate, endDate, null))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Toyota Camry", result.First().Model);
        }
    }
}