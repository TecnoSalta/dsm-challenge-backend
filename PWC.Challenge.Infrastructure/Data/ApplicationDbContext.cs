﻿using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Infrastructure.Data.Extensions;
using System.Reflection;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWC.Challenge.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    private readonly IMediator? _mediator;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IMediator? mediator = null) // Mediator opcional
        : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Rental> Rentals => Set<Rental>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assembly = Assembly.GetExecutingAssembly();
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        modelBuilder.ApplySeedDataFromAssembly(assembly);
        modelBuilder.SetExcludedEntitiesFromMigrations();
        modelBuilder.ApplyGlobalFilter<IEntity>(e => !e.IsDeleted);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        // Guardar cambios primero
        var result = await base.SaveChangesAsync(cancellationToken);

        if (_mediator != null)
        {
            var domainEntities = ChangeTracker.Entries<IHasDomainEvents>()
                .Where(e => e.Entity.DomainEvents != null && e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(e => e.DomainEvents)
                .ToList();

            domainEntities.ForEach(e => e.ClearDomainEvents());

            // Actualización: Envolver cada DomainEvent en un DomainEventNotificationAdapter y publicarlo
            foreach (var domainEvent in domainEvents)
            {
                var adapterType = typeof(PWC.Challenge.Application.Common.DomainEventNotificationAdapter<>).MakeGenericType(domainEvent.GetType());
                var notification = (MediatR.INotification)Activator.CreateInstance(adapterType, domainEvent);
                await _mediator.Publish(notification, cancellationToken);
            }
        }

        return result;
    }
}
