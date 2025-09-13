﻿using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using MediatR;
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Application.Exceptions;
using PWC.Challenge.Application.Features.Rentals.Commands.UpdateRental;
using PWC.Challenge.Application.Features.Rentals.Commands.UpdateRental.Services;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Infrastructure.Data;
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

    private static RentalService CreateRentalService(ApplicationDbContext ctx)
    {
        var rentalRepo = new BaseRepository<Rental>(ctx);
        var carRepo = new BaseRepository<Car>(ctx);

        // Mock MediatR
        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return new RentalService(rentalRepo, carRepo, mediatorMock.Object);
    }

    [Fact]
    public async Task Handle_WhenDatesChangeAndCarIsFree_ShouldUpdate()
    {
        // Arrange
        await using var ctx = new ApplicationDbContext(NewInMemContext());
        var rentalId = Guid.NewGuid();
        var carId = Guid.NewGuid();

        var customer = new Customer(Guid.NewGuid(), "Test User", "123 Test St", "foo@g.com");
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

        var rentalService = CreateRentalService(ctx);
        var handler = new UpdateRentalCommandHandler(rentalService);

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

        var customer = new Customer(Guid.NewGuid(), "Test User", "123 Test St", "foo@g.com");
        ctx.Customers.Add(WithAudit(customer));

        var car = new Car(carId, "Compact", "Mini", 100, CarStatus.Available);
        ctx.Cars.Add(WithAudit(car));

        var otherCustomer = new Customer(Guid.NewGuid(), "Other User", "456 Test Ave", "foo@g.com");
        ctx.Customers.Add(WithAudit(otherCustomer));

        ctx.Rentals.Add(WithAudit(Rental.CreateForTest(Guid.NewGuid(), otherCustomer, car,
                                                       new DateOnly(2025, 10, 6),
                                                       new DateOnly(2025, 10, 10), 40)));
        
        var rentalToUpdate = WithAudit(Rental.CreateForTest(rentalId, customer, car,
                                                            new DateOnly(2025, 10, 1),
                                                            new DateOnly(2025, 10, 5), 40));
        rentalToUpdate.MarkAsActive();
        ctx.Rentals.Add(rentalToUpdate);

        await ctx.SaveChangesAsync();

        var rentalService = CreateRentalService(ctx);
        var handler = new UpdateRentalCommandHandler(rentalService);

        var cmd = new UpdateRentalCommand(
            rentalId,
            new UpdateRentalDto(
                new DateOnly(2025, 10, 6),
                new DateOnly(2025, 10, 8),
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

        var customer = new Customer(Guid.NewGuid(), "Test User", "123 Test St", "foo@g.com");
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
        await rentalEntity.CancelAsync(mediatorMock.Object);
        await ctx.SaveChangesAsync();

        var rentalService = new RentalService(new BaseRepository<Rental>(ctx),
                                              new BaseRepository<Car>(ctx),
                                              mediatorMock.Object);
        var handler = new UpdateRentalCommandHandler(rentalService);

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
}
