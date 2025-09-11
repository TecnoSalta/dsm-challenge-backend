using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Application.Exceptions;
using PWC.Challenge.Application.Features.Commands.Rentals.UpdateReservation;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Infrastructure.Data;
using PWC.Challenge.Infrastructure.Data.Common;

namespace PWC.Challenge.Application.Tests.Features.Rentals.Commands.UpdateReservation;

public class UpdateReservationCommandHandlerTests
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

    [Fact]
    public async Task Handle_WhenDatesChangeAndCarIsFree_ShouldUpdate()
    {
        // Arrange
        await using var ctx = new ApplicationDbContext(NewInMemContext());
        var rentalId = Guid.NewGuid();
        var carId = Guid.NewGuid();

        ctx.Cars.Add(WithAudit(new Car(carId, "Compact", "Mini", CarStatus.Available)));
        ctx.Rentals.Add(WithAudit(new Rental
        {
            Id = rentalId,
            CarId = carId,
            CustomerId = Guid.NewGuid(),
            StartDate = new DateOnly(2025, 10, 1),
            EndDate = new DateOnly(2025, 10, 5),
            Status = RentalStatus.Active
        }));
        await ctx.SaveChangesAsync();

        var handler = new UpdateReservationCommandHandler(
            new BaseRepository<Rental>(ctx),
            new BaseRepository<Car>(ctx)
        );

        var cmd = new UpdateReservationCommand(
            rentalId,
            new UpdateReservationDto(
                new DateOnly(2025, 10, 1),  // Fecha simplificada
                new DateOnly(2025, 10, 10), // Nueva fecha de fin
                null));

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.StartDate.Should().Be(new DateOnly(2025, 10, 1));
        result.EndDate.Should().Be(new DateOnly(2025, 10, 10)); // Corregido para que coincida con el comando
        result.Message.Should().Be("Reservation updated successfully.");
    }

    [Fact]
    public async Task Handle_WhenCarIsNotAvailable_ShouldThrowBusinessException()
    {
        // Arrange
        await using var ctx = new ApplicationDbContext(NewInMemContext());
        var rentalId = Guid.NewGuid();
        var carId = Guid.NewGuid();

        ctx.Cars.Add(WithAudit(new Car(carId, "Compact", "Mini", CarStatus.Available)));

        // Ocupamos el coche en el intervalo objetivo
        ctx.Rentals.Add(WithAudit(new Rental
        {
            Id = Guid.NewGuid(),
            CarId = carId,
            CustomerId = Guid.NewGuid(),
            StartDate = DateOnly.FromDateTime(new DateTime(2025, 10, 6, 0, 0, 0, DateTimeKind.Utc)),
            EndDate = DateOnly.FromDateTime(new DateTime(2025, 10, 10, 0, 0, 0, DateTimeKind.Utc)),
            Status = RentalStatus.Active
        }));

        // Nuestra reserva a modificar
        ctx.Rentals.Add(WithAudit(new Rental
        {
            Id = rentalId,
            CarId = carId,
            CustomerId = Guid.NewGuid(),
            StartDate = DateOnly.FromDateTime(new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc)),
            EndDate = DateOnly.FromDateTime(new DateTime(2025, 10, 5, 0, 0, 0, DateTimeKind.Utc)),
            Status = RentalStatus.Active
        }));
        await ctx.SaveChangesAsync();

        var handler = new UpdateReservationCommandHandler(
            new BaseRepository<Rental>(ctx),
            new BaseRepository<Car>(ctx)
           );

        var cmd = new UpdateReservationCommand(
            rentalId,
            new UpdateReservationDto(
                DateOnly.FromDateTime(new DateTime(2025, 10, 6, 0, 0, 0, DateTimeKind.Utc)), // solapa
                DateOnly.FromDateTime(new DateTime(2025, 10, 8, 0, 0, 0, DateTimeKind.Utc)),
                null));

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(cmd, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenRentalIsNotActive_ShouldThrowBusinessException()
    {
        // Arrange
        await using var ctx = new ApplicationDbContext(NewInMemContext());
        var rentalId = Guid.NewGuid();
        var carId = Guid.NewGuid();

        ctx.Cars.Add(WithAudit(new Car(carId, "Compact", "Mini", CarStatus.Available)));

        ctx.Rentals.Add(WithAudit(new Rental
        {
            Id = rentalId,
            CarId = carId,
            CustomerId = Guid.NewGuid(),
            StartDate = DateOnly.FromDateTime(new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc)),
            EndDate = DateOnly.FromDateTime(new DateTime(2025, 10, 5, 0, 0, 0, DateTimeKind.Utc)),
            Status = RentalStatus.Cancelled // ❌ no activa
        }));
        await ctx.SaveChangesAsync();

        var handler = new UpdateReservationCommandHandler(
            new BaseRepository<Rental>(ctx),
            new BaseRepository<Car>(ctx)
            );

        var cmd = new UpdateReservationCommand(
            rentalId,
            new UpdateReservationDto(
                    DateOnly.FromDateTime(new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc)),
                    DateOnly.FromDateTime(new DateTime(2025, 10, 5, 0, 0, 0, DateTimeKind.Utc)),
                null));

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(cmd, CancellationToken.None));
    }
}