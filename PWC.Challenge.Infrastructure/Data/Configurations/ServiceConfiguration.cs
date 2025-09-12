using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Infrastructure.Data.Configurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Date)
                .IsRequired();

            builder.Property(s => s.DurationDays)
                .IsRequired();

            // Relacion con Car
            builder.HasOne<Car>()
                .WithMany(c => c.Services)
                .HasForeignKey(s => s.CarId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}