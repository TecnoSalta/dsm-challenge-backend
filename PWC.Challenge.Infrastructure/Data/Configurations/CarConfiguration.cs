using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Infrastructure.Data.Configurations
{
    public class CarConfiguration : IEntityTypeConfiguration<Car>
    {
        public void Configure(EntityTypeBuilder<Car> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Model)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.DailyRate)
                .HasPrecision(18, 2);

            builder.Property(c => c.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.HasIndex(c => c.LicensePlate).IsUnique(); // Added unique index for LicensePlate

            // Configure the collection of Services as owned entities
            builder.OwnsMany(c => c.Services, serviceBuilder =>
            {
                serviceBuilder.ToTable("Services");
                serviceBuilder.WithOwner().HasForeignKey("CarId"); // This will be the shadow FK property
                serviceBuilder.Property<Guid>("Id").ValueGeneratedOnAdd(); // Each service instance needs a key
                serviceBuilder.HasKey("Id"); // Set the key for the owned type
            });

            builder.HasMany(c => c.Rentals)
                .WithOne(r => r.Car)
                .HasForeignKey(r => r.CarId);
        }
    }
}