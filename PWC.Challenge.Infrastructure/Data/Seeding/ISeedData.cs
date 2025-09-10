using Microsoft.EntityFrameworkCore;

namespace PWC.Challenge.Infrastructure.Data.Seeding;

public interface ISeedData
{
    int Order { get; }
    void Seed(ModelBuilder modelBuilder);
}
