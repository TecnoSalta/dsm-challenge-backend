using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using MediatR;
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Application.Exceptions;
using PWC.Challenge.Application.Features.Rentals.Commands.UpdateRental;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Infrastructure.Data;
using PWC.Challenge.Domain.Services;
using PWC.Challenge.Infrastructure.Data.Common;

namespace UnitTests.Features.Rentals.Commands.UpdateRental;

public class UpdateRentalCommandHandlerTests
{
    private static DbContextOptions<ApplicationDbContext> NewInMemContext()
        => new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(Guid.NewGuid().ToString())
           .Options;

    private static T WithAudit<T>(T entity) where T : Entity
    {
        entity.CreatedBy = "test";
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedBy = "test";
        entity.UpdatedAt = DateTime.UtcNow;
        return entity;
    }

    // Modified: Renamed and changed return type
    private static (IBaseRepository<Rental> rentalRepo, IBaseRepository<Car> carRepo, IMediator mediator, IRentalAvailabilityService availabilityService) CreateHandlerDependencies(ApplicationDbContext ctx, bool isCarAvailable = true)
    {
        var rentalRepo = new BaseRepository<Rental>(ctx);
        var carRepo = new BaseRepository<Car>(ctx);

        // Mock MediatR
        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var availabilityServiceMock = new Mock<IRentalAvailabilityService>();
        availabilityServiceMock.Setup(s => s.IsCarAvailableAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>())).ReturnsAsync(isCarAvailable);

        return (rentalRepo, carRepo, mediatorMock.Object, availabilityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WhenDatesChangeAndCarIsFree_ShouldUpdate()
    {
        // Arrange
        await using var ctx = new ApplicationDbContext(NewInMemContext());
        var rentalId = Guid.NewGuid();
        var carId = Guid.NewGuid();

        var customer = new Customer(Guid.NewGuid(), "123456789", "Test User", "123 Test St", "foo@g.com");
        ctx.Customers.Add(WithAudit(customer));
        var car = new Car(carId, "Compact", "Mini", 100, CarStatus.Available);
        
        ctx.Cars.Add(WithAudit(car));
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var rental = WithAudit(Rental.CreateForTest(rentalId, customer, car,
                                                    startDate,
                                                    startDate.AddDays(5), 40));
        // To be able to modify, the rental must be active.
        rental.MarkAsActive();
        ctx.Rentals.Add(rental);

        await ctx.SaveChangesAsync();

        // Modified: Get dependencies from helper method
        var (rentalRepo, carRepo, mediator, availabilityService) = CreateHandlerDependencies(ctx);
        var handler = new UpdateRentalCommandHandler(rentalRepo, carRepo, mediator, availabilityService);

        var cmd = new UpdateRentalCommand(
            rentalId,
            new UpdateRentalDto(
                startDate,
                startDate.AddDays(10),
                null
            )
        );

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.StartDate.Should().Be(startDate);
        result.EndDate.Should().Be(startDate.AddDays(10));
        result.Message.Should().Be("Rental updated successfully.");
    }

    [Fact]
    public async Task Handle_WhenCarIsNotAvailable_ShouldThrowBusinessException()
    {
        // Arrange
        await using var ctx = new ApplicationDbContext(NewInMemContext());
        var rentalId = Guid.NewGuid();
        var carId = Guid.NewGuid();

        var customer = new Customer(Guid.NewGuid(), "123456789", "Test User", "123 Test St", "foo@g.com");
        ctx.Customers.Add(WithAudit(customer));

        var car = new Car(carId, "Compact", "Mini", 100, CarStatus.Available);
        ctx.Cars.Add(WithAudit(car));

        var otherCustomer = new Customer(Guid.NewGuid(), "123456789","Other User", "456 Test Ave", "foo@g.com");
        ctx.Customers.Add(WithAudit(otherCustomer));

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        ctx.Rentals.Add(WithAudit(Rental.CreateForTest(Guid.NewGuid(), otherCustomer, car,
                                                       today.AddDays(5),
                                                       today.AddDays(9), 40)));
        
        var rentalToUpdate = WithAudit(Rental.CreateForTest(rentalId, customer, car,
                                                            today,
                                                            today.AddDays(4), 40));
        rentalToUpdate.MarkAsActive();
        ctx.Rentals.Add(rentalToUpdate);

        await ctx.SaveChangesAsync();

        // Modified: Get dependencies from helper method, setting isCarAvailable to false
        var (rentalRepo, carRepo, mediator, availabilityService) = CreateHandlerDependencies(ctx, isCarAvailable: false);
        var handler = new UpdateRentalCommandHandler(rentalRepo, carRepo, mediator, availabilityService);

        var cmd = new UpdateRentalCommand(
            rentalId,
            new UpdateRentalDto(
                today.AddDays(5),
                today.AddDays(7),
                null
            )
        );

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(cmd, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_WhenRentalIsNotActive_ShouldThrowBusinessException()
    {
        // Arrange
        await using var ctx = new ApplicationDbContext(NewInMemContext());
        var rentalId = Guid.NewGuid();
        var carId = Guid.NewGuid();

        var customer = new Customer(Guid.NewGuid(), "123456789", "Test User", "123 Test St", "foo@g.com");
        customer = WithAudit(customer);
        var car = new Car(carId, "Compact", "Mini", 100, CarStatus.Available);

        ctx.Cars.Add(WithAudit(car));
        var rentalEntity = WithAudit(Rental.CreateForTest(rentalId, customer, car,
                                                          new DateOnly(2025, 10, 1),
                                                          new DateOnly(2025, 10, 5), 40));
        ctx.Rentals.Add(rentalEntity);
        await ctx.SaveChangesAsync();

        // We cancel the reservation using CancelAsync with a MediatR mock
        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        // Modified: Call Cancel directly on the entity
        rentalEntity.Cancel();
        await ctx.SaveChangesAsync();

        // Modified: Get dependencies from helper method
        var (rentalRepo, carRepo, mediator, availabilityService) = CreateHandlerDependencies(ctx);
        var handler = new UpdateRentalCommandHandler(rentalRepo, carRepo, mediator, availabilityService);

        var cmd = new UpdateRentalCommand(
            rentalId,
            new UpdateRentalDto(
                new DateOnly(2025, 10, 1),
                new DateOnly(2025, 10, 5),
                null
            )
        );

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(cmd, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_WhenCarIsChangedAndNewCarIsFree_ShouldUpdateRentalAndCarStatus()
    {
        // Arrange
        await using var ctx = new ApplicationDbContext(NewInMemContext());
        var rentalId = Guid.NewGuid();

        var customer = new Customer(Guid.NewGuid(), "123456789", "Test User", "123 Test St", "foo@g.com");
        ctx.Customers.Add(WithAudit(customer));

        var originalCar = new Car(Guid.NewGuid(), "Compact", "Mini", 100, CarStatus.Rented);
        var newCar = new Car(Guid.NewGuid(), "Sedan", "Toyota", 120, CarStatus.Available);
        ctx.Cars.Add(WithAudit(originalCar));
        ctx.Cars.Add(WithAudit(newCar));

        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var rental = WithAudit(Rental.CreateForTest(rentalId, customer, originalCar,
                                                    startDate,
                                                    startDate.AddDays(5), 100));
        rental.MarkAsActive();
        ctx.Rentals.Add(rental);
        await ctx.SaveChangesAsync();

        // Modified: Get dependencies from helper method
        var (rentalRepo, carRepo, mediator, availabilityService) = CreateHandlerDependencies(ctx);
        var handler = new UpdateRentalCommandHandler(rentalRepo, carRepo, mediator, availabilityService);

        var cmd = new UpdateRentalCommand(
            rentalId,
            new UpdateRentalDto(null, null, newCar.Id)
        );

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.CarId.Should().Be(newCar.Id);

        var updatedRental = await ctx.Rentals.FindAsync(rentalId);
        updatedRental.CarId.Should().Be(newCar.Id);

        var updatedOriginalCar = await ctx.Cars.FindAsync(originalCar.Id);
        updatedOriginalCar.Status.Should().Be(CarStatus.Available);

        var updatedNewCar = await ctx.Cars.FindAsync(newCar.Id);
        updatedNewCar.Status.Should().Be(CarStatus.Rented);
    }

    [Fact]
    public async Task Handle_WhenChangingCarAndNewCarIsNotAvailable_ShouldThrowBusinessException()
    {
        // Arrange
        await using var ctx = new ApplicationDbContext(NewInMemContext());
        var rentalId = Guid.NewGuid();

        var customer = new Customer(Guid.NewGuid(), "123456789", "Test User", "123 Test St", "foo@g.com");
        ctx.Customers.Add(WithAudit(customer));

        var originalCar = new Car(Guid.NewGuid(), "Compact", "Mini", 100, CarStatus.Rented);
        var newCar = new Car(Guid.NewGuid(), "Sedan", "Toyota", 120, CarStatus.Available);
        ctx.Cars.Add(WithAudit(originalCar));
        ctx.Cars.Add(WithAudit(newCar));

        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var rental = WithAudit(Rental.CreateForTest(rentalId, customer, originalCar,
                                                    startDate,
                                                    startDate.AddDays(5), 100));
        rental.MarkAsActive();
        ctx.Rentals.Add(rental);
        await ctx.SaveChangesAsync();

        // Modified: Get dependencies from helper method, setting isCarAvailable to false
        var (rentalRepo, carRepo, mediator, availabilityService) = CreateHandlerDependencies(ctx, isCarAvailable: false);
        var handler = new UpdateRentalCommandHandler(rentalRepo, carRepo, mediator, availabilityService);

        var cmd = new UpdateRentalCommand(
            rentalId,
            new UpdateRentalDto(null, null, newCar.Id)
        );

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => handler.Handle(cmd, CancellationToken.None));
    }
}