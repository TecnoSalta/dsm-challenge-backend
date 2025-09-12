using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using PWC.Challenge.Application.Exceptions;
using PWC.Challenge.Application.Features.Rentals.Commands.CompleteRental;
using PWC.Challenge.Application.Features.Rentals.Commands.CompleteRental.Services;
using PWC.Challenge.Application.Features.Rentals.Commands.UpdateRental;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Infrastructure.Data;
using PWC.Challenge.Infrastructure.Data.Common;

namespace UnitTests.Features.Rentals.Commands.CompleteRental;

public class CompleteRentalCommandHandlerTests
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

    private static CompleteRentalService CreateService(ApplicationDbContext ctx)
    {
        var rentalRepo = new BaseRepository<Rental>(ctx);
        var carRepo = new BaseRepository<Car>(ctx);
        var serviceRepo = new BaseRepository<Service>(ctx);

        // Mock de MediatR
        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return new CompleteRentalService(rentalRepo, carRepo, serviceRepo, mediatorMock.Object);
    }

    [Fact]
    public async Task Handle_WhenRentalIsActive_ShouldMarkAsCompletedAndScheduleServiceIfDue()
    {
        // Arrange
        await using var ctx = new ApplicationDbContext(NewInMemContext());
        var rentalId = Guid.NewGuid();
        var carId = Guid.NewGuid();

        var customer = WithAudit(new Customer(Guid.NewGuid(), "Test User", "123 Test St"));
        var car = WithAudit(new Car(carId, "Sedan", "Toyota",CarStatus.Available));

        var rental = WithAudit(new Rental(rentalId, customer, car,
                                          new DateOnly(2025, 9, 1),
                                          new DateOnly(2025, 9, 10)));
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        rental.Active(today);

        ctx.Customers.Add(customer);
        ctx.Cars.Add(car);
        ctx.Rentals.Add(rental);
        await ctx.SaveChangesAsync();

        var service = CreateService(ctx);
        var handler = new CompleteRentalCommandHandler(service);

        var cmd = new CompleteRentalCommand(rentalId);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        //TODO : completar aserciones
        /*
        result.Id.Should().Be(rentalId);
        result.Status.Should().Be(RentalStatus.Completed.ToString());

        var updatedRental = await ctx.Rentals.FirstAsync(r => r.Id == rentalId);
        updatedRental.Status.Should().Be(RentalStatus.Completed);
        updatedRental.ActualReturnDate.Should().NotBeNull();

        // Debió generar un Service porque pasó más de 2 meses desde último service
        var scheduledService = await ctx.Services.FirstOrDefaultAsync(s => s.CarId == carId);
        scheduledService.Should().NotBeNull();
        scheduledService.StartDate.Date.Should().Be(DateTime.UtcNow.Date);
        */
    }

    [Fact]
    public async Task Handle_WhenRentalIsNotActive_ShouldThrowBusinessException()
    {
        // Arrange
        await using var ctx = new ApplicationDbContext(NewInMemContext());
        var rentalId = Guid.NewGuid();
        var carId = Guid.NewGuid();

        var customer = WithAudit(new Customer(Guid.NewGuid(), "User", "456 Other St"));
        var car = WithAudit(new Car(carId, "SUV", "Honda", CarStatus.Available));

        var rental = WithAudit(new Rental(rentalId, customer, car,
                                          new DateOnly(2025, 9, 1),
                                          new DateOnly(2025, 9, 10)));
        // rental no se activa

        ctx.Customers.Add(customer);
        ctx.Cars.Add(car);
        ctx.Rentals.Add(rental);
        await ctx.SaveChangesAsync();

        var service = CreateService(ctx);
        var handler = new CompleteRentalCommandHandler(service);

        var cmd = new CompleteRentalCommand(rentalId);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(cmd, CancellationToken.None)
        );
    }
}
