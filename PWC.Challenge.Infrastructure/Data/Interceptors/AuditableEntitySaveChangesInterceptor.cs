using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Application.Interfaces;
using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Infrastructure.Data.Interceptors;

public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;

    public AuditableEntitySaveChangesInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries<Entity>() )
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = _currentUserService.UserId ?? "System"; // Fallback to System if no user
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedBy = _currentUserService.UserId ?? "System";
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
