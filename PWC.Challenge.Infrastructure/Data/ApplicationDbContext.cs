using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Domain.Cars;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Infrastructure.Data.Extensions;
using System.Reflection;

namespace PWC.Challenge.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options
        ) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Service> Services => Set<Service>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assembly = Assembly.GetExecutingAssembly();
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        modelBuilder.ApplySeedDataFromAssembly(assembly);
        modelBuilder.SetExcludedEntitiesFromMigrations();
        modelBuilder.ApplyGlobalFilter<IEntity>(e => !e.IsDeleted);
        base.OnModelCreating(modelBuilder);
    }
}
