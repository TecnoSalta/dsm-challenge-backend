﻿using Microsoft.EntityFrameworkCore;
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

        var rentalId = Guid.Parse("22222222-2222-2222-2222-222222222221");

        modelBuilder.Entity<Rental>().HasData(
            new
            {
                Id = rentalId,
                CustomerId = Customer.FooId,
                CarId = Car.Car1Id,
                CreatedAt = createdAt,
                CreatedBy = createdBy,
                IsDeleted = false,
                Status = RentalStatus.Active,
                DailyRate = 100m,
                TotalCost = 1260m
            });

        modelBuilder.Entity<Rental>().OwnsOne(r => r.RentalPeriod).HasData(
            new { RentalId = rentalId, StartDate = new DateOnly(2025, 1, 1), EndDate = new DateOnly(2025, 1, 15) });
    }
}
