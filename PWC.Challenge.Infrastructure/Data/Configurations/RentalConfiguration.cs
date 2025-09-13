using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Infrastructure.Data.Configurations
{
    public class RentalConfiguration : IEntityTypeConfiguration<Rental>
    {
        public void Configure(EntityTypeBuilder<Rental> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.OwnsOne(r => r.RentalPeriod, periodBuilder =>
            {
                periodBuilder.Property(p => p.StartDate).HasColumnName("StartDate");
                periodBuilder.Property(p => p.EndDate).HasColumnName("EndDate");
            });
        }
    }
}