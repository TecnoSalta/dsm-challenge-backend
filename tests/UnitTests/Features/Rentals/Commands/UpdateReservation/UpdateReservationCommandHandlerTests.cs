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
        var customer = new Customer(
                Guid.NewGuid(),
                fullName: "Test User",
                address: "123 Test St"
            )
        {
            CreatedBy = "test",
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = "test",
            UpdatedAt = DateTime.UtcNow
        };
        var car = new Car(carId, "Compact", "Mini", CarStatus.Available);

        ctx.Cars.Add(WithAudit(car));
        ctx.Rentals.Add(WithAudit(new Rental(
            rentalId,
            customer,
            car,
            new DateOnly(2025, 10, 1),
            new DateOnly(2025, 10, 5)
        )));
        await ctx.SaveChangesAsync();

        var handler = new UpdateReservationCommandHandler(
            new BaseRepository<Rental>(ctx),
            new BaseRepository<Car>(ctx)
        );

        var cmd = new UpdateReservationCommand(
            rentalId,
            new UpdateReservationDto(
                new DateOnly(2025, 10, 1),
                new DateOnly(2025, 10, 10),
                null
            )
        );

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.StartDate.Should().Be(new DateOnly(2025, 10, 1));
        result.EndDate.Should().Be(new DateOnly(2025, 10, 10));
        result.Message.Should().Be("Reservation updated successfully.");
    }

    [Fact]
    public async Task Handle_WhenCarIsNotAvailable_ShouldThrowBusinessException()
    {
        // Arrange
        await using var ctx = new ApplicationDbContext(NewInMemContext());
        var rentalId = Guid.NewGuid();
        var carId = Guid.NewGuid();

        // Customer principal de la reserva que vamos a modificar
        var customer = new Customer(
            Guid.NewGuid(),
            fullName: "Test User",
            address: "123 Test St"
        )
        {
            CreatedBy = "test",
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = "test",
            UpdatedAt = DateTime.UtcNow
        };

        // Coche que se va a reservar
        var car = new Car(carId, "Compact", "Mini", CarStatus.Available);

        ctx.Cars.Add(WithAudit(car));

        var otherCustomer = new Customer(
            Guid.NewGuid(),
            fullName: "Other User",
            address: "456 Test Ave"
        )
        {
            CreatedBy = "test",
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = "test",
            UpdatedAt = DateTime.UtcNow
        };

        // Reserva ocupando el coche en el intervalo objetivo
        ctx.Rentals.Add(WithAudit(new Rental(
            Guid.NewGuid(),
            otherCustomer, // ahora un Customer completo
            car,
            new DateOnly(2025, 10, 6),
            new DateOnly(2025, 10, 10)
        )));

        // Nuestra reserva a modificar
        ctx.Rentals.Add(WithAudit(new Rental(
            rentalId,
            customer,
            car,
            new DateOnly(2025, 10, 1),
            new DateOnly(2025, 10, 5)
        )));

        // Guardamos todo en InMemoryDatabase
        await ctx.SaveChangesAsync();

        var handler = new UpdateReservationCommandHandler(
            new BaseRepository<Rental>(ctx),
            new BaseRepository<Car>(ctx)
        );

        var cmd = new UpdateReservationCommand(
            rentalId,
            new UpdateReservationDto(
                new DateOnly(2025, 10, 6), // solapa con otra reserva
                new DateOnly(2025, 10, 8),
                null
            )
        );

        // Act & Assert
        // Ahora EF no falla porque todos los Customer tienen FullName y Address
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

        // Customer completo para EF Core (propiedades requeridas)
        var customer = new Customer(
            Guid.NewGuid(),
            fullName: "Test User",
            address: "123 Test St"
        )
        {
            CreatedBy = "test",
            CreatedAt = DateTime.UtcNow,
            UpdatedBy = "test",
            UpdatedAt = DateTime.UtcNow
        };

        // Coche disponible
        var car = new Car(carId, "Compact", "Mini", CarStatus.Available);

        ctx.Cars.Add(WithAudit(car));

        // Reserva inicial
        ctx.Rentals.Add(WithAudit(new Rental(
            rentalId,
            customer,
            car,
            new DateOnly(2025, 10, 1),
            new DateOnly(2025, 10, 5)
        )));

        await ctx.SaveChangesAsync();

        var rental = await ctx.Rentals.FirstAsync(r => r.Id == rentalId);
        rental.Cancel(); // Cambia status a Cancelled
        await ctx.SaveChangesAsync();

        var handler = new UpdateReservationCommandHandler(
            new BaseRepository<Rental>(ctx),
            new BaseRepository<Car>(ctx)
        );

        var cmd = new UpdateReservationCommand(
            rentalId,
            new UpdateReservationDto(
                new DateOnly(2025, 10, 1),
                new DateOnly(2025, 10, 5),
                null
            )
        );

        // Act & Assert
        // EF ya no falla porque Customer tiene FullName y Address
        // Se lanza BusinessException porque rental no está activo
        await Assert.ThrowsAsync<BusinessException>(
            () => handler.Handle(cmd, CancellationToken.None)
        );
    }

}
