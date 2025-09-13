using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PWC.Challenge.Infrastructure.Data.Seeding;
using System.Linq.Expressions;
using System.Reflection;

namespace PWC.Challenge.Infrastructure.Data.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplySeedDataFromAssembly(this ModelBuilder modelBuilder, Assembly assembly)
    {
        var seedDataTypes = assembly.GetTypes()
            .Where(t => typeof(ISeedData).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .OrderBy(t =>
            {
                var instance = Activator.CreateInstance(t) as ISeedData;
                return instance?.Order ?? int.MaxValue;
            });

        foreach (var type in seedDataTypes)
        {
            var instance = Activator.CreateInstance(type);
            if (instance is ISeedData seedDataInstance)
                seedDataInstance.Seed(modelBuilder);
        }

    }

    public static void SetExcludedEntitiesFromMigrations(this ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<PutYourEntityNameHere>().Metadata.SetIsTableExcludedFromMigrations(true);
    }

    public static void ApplyGlobalFilter<TInterface>(this ModelBuilder modelBuilder, Expression<Func<TInterface, bool>> filter)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Ignorar tipos owned para evitar conflictos
            if (entityType.IsOwned() == false && typeof(TInterface).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var body = ReplacingExpressionVisitor.Replace(filter.Parameters.Single(), parameter, filter.Body);
                var lambdaExpression = Expression.Lambda(body, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambdaExpression);
            }
        }
    }
}
