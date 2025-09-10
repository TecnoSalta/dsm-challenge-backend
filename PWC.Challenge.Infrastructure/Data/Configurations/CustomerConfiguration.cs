
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWC.Challenge.Domain.Entities;
namespace PWC.Challenge.Infrastructure.Data.Configurations;



public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.RegistrationDate)
            .IsRequired();

        // Índice único para IdentityId
        builder.HasIndex(c => c.IdentityId)
            .IsUnique();

    }
}
