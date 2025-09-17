using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Enums;

namespace PWC.Challenge.Infrastructure.Data.Seeding;

public class CarSeed : ISeedData
{
    public int Order => 2;

    public void Seed(ModelBuilder modelBuilder)
    {
        var createdAt = DateTime.MinValue;
        var createdBy = "Anonymous";

        var fooCar = new Car(
            Car.Car1Id,
            "camioneta", "Hilux", 100, "AB753BG", CarStatus.Available // Added LicensePlate
        )
        {
            CreatedAt = createdAt,
            CreatedBy = createdBy
        };
        var fooCar2 = new Car(
           Car.Car2Id,
           "auto", "Explorer", 100, "AB753BD", CarStatus.Available // Added LicensePlate
       )
        {
            CreatedAt = createdAt,
            CreatedBy = createdBy
        };
        var fooCar3 = new Car(
           Car.Car3Id,
           "auto", "Mini", 100, "AB753BY", CarStatus.Available // Added LicensePlate
       )
        {
            CreatedAt = createdAt,
            CreatedBy = createdBy
        };
        modelBuilder.Entity<Car>().HasData(
            fooCar, fooCar2, fooCar3
        );
    }
}