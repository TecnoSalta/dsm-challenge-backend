using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Infrastructure.Data.Seeding;

public class CustomerSeed : ISeedData
{
    public int Order => 1;

    public void Seed(ModelBuilder modelBuilder)
    {
        var createdAt = DateTime.MinValue;
        var createdBy = "Anonymous";

        var fooCustomer = new Customer(
            Customer.FooId,
            "SuperAdmin", "Address 1234"
        )
        {
            CreatedAt = createdAt,
            CreatedBy = createdBy
        };

        modelBuilder.Entity<Customer>().HasData(
            fooCustomer
        );
    }
}
