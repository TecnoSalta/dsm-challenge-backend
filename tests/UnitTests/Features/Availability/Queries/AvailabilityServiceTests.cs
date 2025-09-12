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
        private readonly Mock<IServiceRepository> _serviceRepositoryMock;
        private readonly AvailabilityService _service;

        public AvailabilityServiceTests()
        {
            _carRepositoryMock = new Mock<ICarRepository>();
            _rentalRepositoryMock = new Mock<IRentalRepository>();
            _serviceRepositoryMock = new Mock<IServiceRepository>();

            _service = new AvailabilityService(
                _carRepositoryMock.Object,
                _rentalRepositoryMock.Object,
                _serviceRepositoryMock.Object);
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

            var car = new Car(carId, "Sedan", "Toyota Camry", 50m);
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

            var car = new Car(carId, "Sedan", "Toyota Camry", 50m);

            _carRepositoryMock
              .Setup(repo => repo.GetByIdAsync(carId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
              .ReturnsAsync(car);


            // Usar valores explícitos en lugar de parámetros opcionales
            _serviceRepositoryMock.Setup(repo =>
                repo.HasScheduledServicesAsync(carId, startDate, endDate))
                .ReturnsAsync(true);

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

            var car = new Car(carId, "Sedan", "Toyota Camry", 50m);

            _carRepositoryMock
            .Setup(repo => repo.GetByIdAsync(carId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(car);


            // Usar valores explícitos
            _serviceRepositoryMock.Setup(repo =>
                repo.HasScheduledServicesAsync(carId, startDate, endDate))
                .ReturnsAsync(false);

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

            var car = new Car(carId, "Sedan", "Toyota Camry", 50m);
            var previousDay = startDate.AddDays(-1);
            _carRepositoryMock
            .Setup(repo => repo.GetByIdAsync(carId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(car);

            // Usar valores explícitos
            _serviceRepositoryMock.Setup(repo =>
                repo.HasScheduledServicesAsync(carId, startDate, endDate))
                .ReturnsAsync(false);

            // Usar null explícito
            _rentalRepositoryMock.Setup(repo =>
                repo.HasOverlappingRentalsAsync(carId, startDate, endDate, null))
                .ReturnsAsync(false);

            // Usar null explícito y fechas específicas
            _rentalRepositoryMock.Setup(repo =>
                repo.GetOverlappingRentalsAsync(carId, previousDay, previousDay, null))
                .ReturnsAsync(new List<Rental> { new Rental() });

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

            var car = new Car(carId, "Sedan", "Toyota Camry", 50m);

            _carRepositoryMock
             .Setup(repo => repo.GetByIdAsync(carId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(car);
            // Usar valores explícitos
            _serviceRepositoryMock.Setup(repo =>
                repo.HasScheduledServicesAsync(carId, startDate, endDate))
                .ReturnsAsync(false);

            // Usar null explícito
            _rentalRepositoryMock.Setup(repo =>
                repo.HasOverlappingRentalsAsync(carId, startDate, endDate, null))
                .ReturnsAsync(false);

            // Usar null explícito
            _rentalRepositoryMock.Setup(repo =>
                repo.GetOverlappingRentalsAsync(carId, previousDay, previousDay, null))
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

            var car = new Car(carId, "Sedan", "Toyota Camry", 50m);

            _carRepositoryMock
                .Setup(repo => repo.GetByIdAsync(carId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(car);

            // Usar valores explícitos
            _serviceRepositoryMock.Setup(repo =>
                repo.HasScheduledServicesAsync(carId, startDate, endDate))
                .ReturnsAsync(false);

            // Usar excludedRentalId explícito
            _rentalRepositoryMock.Setup(repo =>
                repo.HasOverlappingRentalsAsync(carId, startDate, endDate, excludedRentalId))
                .ReturnsAsync(false);

            // Usar excludedRentalId explícito
            _rentalRepositoryMock.Setup(repo =>
                repo.GetOverlappingRentalsAsync(carId, previousDay, previousDay, excludedRentalId))
                .ReturnsAsync(new List<Rental>());

            // Act
            var result = await _service.IsCarAvailableAsync(carId, startDate, endDate, excludedRentalId);

            // Assert
            Assert.True(result);
        }
    }
}