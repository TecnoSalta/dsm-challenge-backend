// GetAvailableCarsQueryHandlerTests.cs
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Application.Features.Querys.GetAvailableCars;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Rentals;
using PWC.Challenge.Infrastructure.Data;
using PWC.Challenge.Infrastructure.Data.Common;

namespace UnitTests.Features.Availability.Queries;

public class GetAvailableCarsQueryHandlerTests
{
    private static DbContextOptions<ApplicationDbContext> NewInMemContext()
    {
        var name = Guid.NewGuid().ToString();   // base limpia por test
        return new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(name)
               .Options;
    }

    [Fact]
    public async Task Handle_WhenCarIsBooked_ShouldNotReturnIt()
    {
        // Arrange
        await using var ctx = new ApplicationDbContext(NewInMemContext());

        var carId = Guid.NewGuid();
        var car = new Car(carId, "Toyota", "Corolla", "unavailable")
        {
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "test"
        };
        ctx.Cars.Add(car);
        
        await ctx.SaveChangesAsync();

        var carRepo = new BaseRepository<Car>(ctx);
        var serviceRepo = new BaseRepository<Service>(ctx);

        var handler = new GetAvailableCarsQueryHandler(carRepo);
        var query = new GetAvailableCarsQuery(
            new AvailabilityQueryDto(
                new DateTime(2025, 9, 12),
                new DateTime(2025, 9, 14),
                null, null));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenCarIsFree_ShouldReturnIt()
    {
        //TODO
        // Arrange
        /*await using var ctx = new ApplicationDbContext(NewInMemContext());
        var carId = Guid.NewGuid();
        ctx.Cars.Add(new Car(carId,"Mini","Expo","available")
        )
        await ctx.SaveChangesAsync();

        var carRepo = new Repository<Car>(ctx);
        var rentalRepo = new Repository<Rental>(ctx);
        var serviceRepo = new Repository<Service>(ctx);

        var handler = new GetAvailableCarsQueryHandler(carRepo, rentalRepo, serviceRepo);
        var query = new GetAvailableCarsQuery(
            new AvailabilityQueryDto(
                new DateTime(2025, 10, 1),
                new DateTime(2025, 10, 5),
                null, null));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().ContainSingle();
        result.Single().CarId.Should().Be(carId);*/
    }

    [Fact]
    public async Task Handle_WhenCarHasScheduledService_ShouldNotReturnIt()
    {
        /*
        // Arrange
        await using var ctx = new ApplicationDbContext(NewInMemContext());
        var carId = Guid.NewGuid();
        ctx.Cars.Add(new Car
        {
            Id = carId,
            LicensePlate = "SERV1",
            Model = "Focus",
            CarType = "Compact",
            Status = CarStatus.InService,
            DailyPrice = 45
        });
        ctx.Services.Add(new Service
        {
            Id = Guid.NewGuid(),
            CarId = carId,
            StartDate = new DateTime(2025, 9, 20),
            EndDate = new DateTime(2025, 9, 22)
        });
        await ctx.SaveChangesAsync();

        var carRepo = new Repository<Car>(ctx);
        var rentalRepo = new Repository<Rental>(ctx);
        var serviceRepo = new Repository<Service>(ctx);

        var handler = new GetAvailableCarsQueryHandler(carRepo, rentalRepo, serviceRepo);
        var query = new GetAvailableCarsQuery(
            new AvailabilityQueryDto(
                new DateTime(2025, 9, 21),
                new DateTime(2025, 9, 23),
                null, null));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
        */
    }
}
