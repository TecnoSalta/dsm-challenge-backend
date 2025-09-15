using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Domain.Entities;
using System.Net;

namespace PWC.Challenge.Infrastructure.Data.Seeding;

public class CustomerSeed : ISeedData
{
    public int Order => 1;

    public void Seed(ModelBuilder modelBuilder)
    {
        var createdAt = DateTime.MinValue;
        var createdBy = "Anonymous";
   
        var fooCustomer = new Customer(
                Customer.FooId,"123456789",
                "SuperAdmin", "Address 1234","foo@g.com"
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
