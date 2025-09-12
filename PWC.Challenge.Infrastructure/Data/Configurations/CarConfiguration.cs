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

            // Configurar la colección de Services como owned entities
            builder.OwnsMany(c => c.Services, serviceBuilder =>
            {
                serviceBuilder.WithOwner().HasForeignKey("CarId");
                serviceBuilder.Property<Guid>("Id");
                serviceBuilder.HasKey("Id");
                serviceBuilder.Property(s => s.Date).IsRequired();
                serviceBuilder.Property(s => s.DurationDays).IsRequired();
            });

            builder.HasMany(c => c.Rentals)
                .WithOne(r => r.Car)
                .HasForeignKey(r => r.CarId);
        }
    }
}