using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Domain.Cars;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Rentals;

namespace PWC.Challenge.Infrastructure.Data;
public class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSets para cada entidad
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<Service> Services { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar todas las configuraciones de entidades
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Configuraciones adicionales
        ConfigureModel(modelBuilder);
    }

    private void ConfigureModel(ModelBuilder modelBuilder)
    {
        // Configurar convenciones globales
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Configurar nombres de tablas en plural
            if (entityType.ClrType.IsSubclassOf(typeof(Entity)))
            {
                modelBuilder.Entity(entityType.ClrType).ToTable(entityType.ClrType.Name + "s");
            }

            // Configurar propiedades de auditoría si las tienes
            if (typeof(IAuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<DateTime>("CreatedAt")
                    .IsRequired();

                modelBuilder.Entity(entityType.ClrType)
                    .Property<DateTime?>("UpdatedAt");
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Procesar eventos de dominio antes de guardar
        await DispatchDomainEventsAsync();

        // Actualizar propiedades de auditoría
        UpdateAuditableEntities();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditableEntities()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is IAuditableEntity &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (IAuditableEntity)entry.Entity;
            var now = DateTime.UtcNow;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = now;
            }
            else
            {
                Entry(entity).Property(x => x.CreatedAt).IsModified = false;
            }

            entity.UpdatedAt = now;
        }
    }

    private async Task DispatchDomainEventsAsync()
    {
        var domainEventEntities = ChangeTracker.Entries<Entity>()
            .Select(po => po.Entity)
            .Where(po => po.DomainEvents.Any())
            .ToArray();

        foreach (var entity in domainEventEntities)
        {
            var events = entity.DomainEvents.ToArray();
            entity.ClearDomainEvents();

            // Aquí podrías publicar los eventos si usas MediatR
            // foreach (var domainEvent in events)
            // {
            //     await _mediator.Publish(domainEvent);
            // }
        }
    }

    // Implementación de IUnitOfWork
    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task BeginTransactionAsync()
    {
        await Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await Database.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await Database.RollbackTransactionAsync();
    }
}
