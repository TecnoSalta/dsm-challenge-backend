using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Infrastructure.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasOne(c => c.User)
               .WithOne()
               .HasForeignKey<Customer>(c => c.UserId)
               .IsRequired();

        builder.HasIndex(c => c.Dni).IsUnique(); // Added unique index for Dni
    }
}