using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;

namespace PWC.Challenge.Infrastructure.Data.Seeding;

public class RentalSeed : ISeedData
{
    public int Order => 2;

    public void Seed(ModelBuilder modelBuilder)
    {
        var createdAt = DateTime.MinValue;
        var createdBy = "Anonymous";

        modelBuilder.Entity<Rental>().HasData(new
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222221"),
            CustomerId = Customer.FooId,
            CarId = Car.Car1Id,
            StartDate = new DateOnly(2025, 01, 01),
            EndDate = new DateOnly(2025, 01, 15),
            CreatedAt = createdAt,
            CreatedBy = createdBy,
            IsDeleted = false,
            Status =RentalStatus.Active
        });
    }
}
