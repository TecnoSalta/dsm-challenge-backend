using Moq;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Domain.Interfaces;
using PWC.Challenge.Domain.Services;

namespace PWC.Challenge.UnitTests.Domain.Services
{
    public class AvailabilityServiceTests
    {
        private readonly Mock<ICarRepository> _carRepositoryMock;
        private readonly Mock<IRentalRepository> _rentalRepositoryMock;
        private readonly AvailabilityService _service;

        public AvailabilityServiceTests()
        {
            _carRepositoryMock = new Mock<ICarRepository>();
            _rentalRepositoryMock = new Mock<IRentalRepository>();

            _service = new AvailabilityService(
                _carRepositoryMock.Object,
                _rentalRepositoryMock.Object);
        }

        [Fact]
        public async Task IsCarAvailableAsync_ShouldReturnFalse_WhenCarNotFound()
        {
            // Arrange
            var carId = Guid.NewGuid();
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3));


            _carRepositoryMock
            .Setup(repo => repo.GetByIdAsync(carId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Car?)null);


            // Act
            var result = await _service.IsCarAvailableAsync(carId, startDate, endDate);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsCarAvailableAsync_ShouldReturnFalse_WhenCarInMaintenance()
        {
            // Arrange
            var carId = Guid.NewGuid();
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3));

            var car = new Car(carId, "Sedan", "Toyota Camry", 50m, "TESTPLATE");
            car.MarkAsInMaintenance();

            _carRepositoryMock
                .Setup(repo => repo.GetByIdAsync(carId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(car);
            // Act
            var result = await _service.IsCarAvailableAsync(carId, startDate, endDate);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsCarAvailableAsync_ShouldReturnFalse_WhenServiceScheduled()
        {
            // Arrange
            var carId = Guid.NewGuid();
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3));

            var car = new Car(carId, "Sedan", "Toyota Camry", 50m, "TESTPLATE", CarStatus.Available);
            car.AddService(startDate, 2); // Add a service that overlaps

            _carRepositoryMock
              .Setup(repo => repo.GetByIdAsync(carId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
              .ReturnsAsync(car);

            // Act
            var result = await _service.IsCarAvailableAsync(carId, startDate, endDate);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsCarAvailableAsync_ShouldReturnFalse_WhenOverlappingRentalExists()
        {
            // Arrange
            var carId = Guid.NewGuid();
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3));

            var car = new Car(carId, "Sedan", "Toyota Camry", 50m, "TESTPLATE");

            _carRepositoryMock
            .Setup(repo => repo.GetByIdAsync(carId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(car);


            // Usar null explícito para excludedRentalId
            _rentalRepositoryMock.Setup(repo =>
                repo.HasOverlappingRentalsAsync(carId, startDate, endDate, null))
                .ReturnsAsync(true);

            // Act
            var result = await _service.IsCarAvailableAsync(carId, startDate, endDate);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsCarAvailableAsync_ShouldReturnFalse_WhenRentalEndsDayBefore()
        {
            // Arrange
            var carId = Guid.NewGuid();
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3));

            var car = new Car(carId, "Sedan", "Toyota Camry", 50m, "TESTPLATE");
            var previousDay = startDate.AddDays(-1);
            _carRepositoryMock
            .Setup(repo => repo.GetByIdAsync(carId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(car);

            // Usar null explícito
            _rentalRepositoryMock.Setup(repo =>
                repo.HasOverlappingRentalsAsync(carId, startDate, endDate, null))
                .ReturnsAsync(false);

            _rentalRepositoryMock.Setup(repo =>
                repo.GetRentalsByEndDateAsync(carId, previousDay, null))
                .ReturnsAsync(() =>
                {
                    var customer = new Customer(Guid.NewGuid(), "123456789", "Test", "Test", "test@test.com");
                    var rentalCar = new Car(carId, "Sedan", "Toyota Camry", 50m, "RENTALPLATE");
                    var rental = Rental.CreateForTest(
                        Guid.NewGuid(),
                        customer,
                        rentalCar,
                        previousDay.AddDays(-5), // Start date in the past
                        previousDay, // End date is the previous day
                        50m);
                    return new List<Rental> { rental };
                });

            // Act
            var result = await _service.IsCarAvailableAsync(carId, startDate, endDate);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsCarAvailableAsync_ShouldReturnTrue_WhenCarAvailable()
        {
            // Arrange
            var carId = Guid.NewGuid();
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3));
            var previousDay = startDate.AddDays(-1);

            var car = new Car(carId, "Sedan", "Toyota Camry", 50m, "TESTPLATE");

            _carRepositoryMock
             .Setup(repo => repo.GetByIdAsync(carId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(car);

            // Usar null explícito
            _rentalRepositoryMock.Setup(repo =>
                repo.HasOverlappingRentalsAsync(carId, startDate, endDate, null))
                .ReturnsAsync(false);

            _rentalRepositoryMock.Setup(repo =>
                repo.GetRentalsByEndDateAsync(carId, previousDay, null))
                .ReturnsAsync(new List<Rental>());

            // Act
            var result = await _service.IsCarAvailableAsync(carId, startDate, endDate);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsCarAvailableAsync_ShouldReturnTrue_WhenExcludedRentalIdProvided()
        {
            // Arrange
            var carId = Guid.NewGuid();
            var excludedRentalId = Guid.NewGuid();
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3));
            var previousDay = startDate.AddDays(-1);

            var car = new Car(carId, "Sedan", "Toyota Camry", 50m, "TESTPLATE");

            _carRepositoryMock
                .Setup(repo => repo.GetByIdAsync(carId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(car);

            // Usar excludedRentalId explícito
            _rentalRepositoryMock.Setup(repo =>
                repo.HasOverlappingRentalsAsync(carId, startDate, endDate, excludedRentalId))
                .ReturnsAsync(false);

            _rentalRepositoryMock.Setup(repo =>
                repo.GetRentalsByEndDateAsync(carId, previousDay, excludedRentalId))
                .ReturnsAsync(new List<Rental>());

            // Act
            var result = await _service.IsCarAvailableAsync(carId, startDate, endDate, excludedRentalId);

            // Assert
            Assert.True(result);
        }
    }
}