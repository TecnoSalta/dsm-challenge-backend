using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;

namespace PWC.Challenge.Infrastructure.Data.Extensions;

public static class PersistenceExtensions
{
    public static async Task ApplyDatabaseMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        //context.Database.MigrateAsync().GetAwaiter().GetResult();
        await context.Database.MigrateAsync();
    }

    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            r.TargetEntry.State == EntityState.Modified);

    public static IQueryable<TEntity> AsNoTrackingIf<TEntity>(this IQueryable<TEntity> query, bool asNoTracking)
        where TEntity : class
    {
        return asNoTracking ? query.AsNoTracking() : query;
    }
}
