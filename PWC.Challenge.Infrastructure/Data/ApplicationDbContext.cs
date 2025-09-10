using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Infrastructure.Data.Extensions;
using System.Reflection;

namespace PWC.Challenge.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    private readonly IDistributedCache cache;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IDistributedCache cache) : base(options)
    {
        this.cache = cache;
    }

    public DbSet<Customer> Customers => Set<Customer>();
    

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
