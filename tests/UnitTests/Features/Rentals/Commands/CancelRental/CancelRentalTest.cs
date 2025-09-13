﻿using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PWC.Challenge.Application.Features.Cars.EventHandlers;
using PWC.Challenge.Application.Features.Rentals.Commands.CancelRental;
using PWC.Challenge.Application.Features.Rentals.Commands.UpdateRental.Services;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;
using PWC.Challenge.Infrastructure.Data;
using PWC.Challenge.Infrastructure.Data.Common;

namespace UnitTests.Features.Rentals.Commands.CancelRental;

public class CancelRentalCommandHandlerIntegrationTests
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
    public async Task Handle_WhenRentalIsCancelled_ShouldSetRentalToCancelled_AndCarToAvailable()
    {
        // Arrange: ServiceProvider con MediatR
        var services = new ServiceCollection();

        // Agregar logging necesario para MediatR
        services.AddLogging();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        // Repositorios genéricos
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

        // Registrar MediatR y handlers
        var assembly = typeof(MakeCarAvailableOnRentalCancelledHandler).Assembly;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // Registrar RentalService
        services.AddScoped<RentalService>();

        var provider = services.BuildServiceProvider();

        var ctx = provider.GetRequiredService<ApplicationDbContext>();

        var rentalId = Guid.NewGuid();
        var carId = Guid.NewGuid();
        var customer = new Customer(Guid.NewGuid(), "Test User", "123 Test St", "foo@g.com");
        customer = WithAudit(customer);

        var car = new Car(carId, "Compact", "Mini", 100,CarStatus.Rented);

        ctx.Cars.Add(WithAudit(car));
        ctx.Rentals.Add(WithAudit(Rental.CreateForTest(
            rentalId,
            customer,
            car,
            new DateOnly(2025, 10, 1),
            new DateOnly(2025, 10, 5), 40
        )));
        await ctx.SaveChangesAsync();

        var rentalService = provider.GetRequiredService<RentalService>();
        var handler = new CancelRentalCommandHandler(rentalService);

        var cmd = new CancelRentalCommand(rentalId);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();

        var cancelledRental = await ctx.Rentals.FirstAsync(r => r.Id == rentalId);
        cancelledRental.Status.Should().Be(RentalStatus.Cancelled);

        var updatedCar = await ctx.Cars.FirstAsync(c => c.Id == carId);
        updatedCar.Status.Should().Be(CarStatus.Available);
    }
}
