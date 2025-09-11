using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using PWC.Challenge.Application.Exceptions;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Infrastructure.Data;
using PWC.Challenge.Infrastructure.Data.Extensions;
using System.Linq.Expressions;

namespace PWC.Challenge.Infrastructure.Data.Common;

public class BaseRepository<TEntity> : Repository, IBaseRepository<TEntity>
    where TEntity : Entity
   
{
    public BaseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        await Context.Set<TEntity>().AddAsync(entity, cancellationToken);
        if (saveChanges)
            await SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        Context.Set<TEntity>().AddRange(entities);
        if (saveChanges)
            await SaveChangesAsync(cancellationToken);
    }

    public virtual async Task BulkAddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await Context.BulkInsertAsync(entities, bulkConfig: null, progress: null, type: null, cancellationToken: cancellationToken);
    }

    public virtual async Task BulkDeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await Context.BulkDeleteAsync(entities, bulkConfig: null, progress: null, type: null, cancellationToken: cancellationToken);
    }

    public virtual async Task BulkSaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await Context.BulkSaveChangesAsync(bulkConfig: null, progress: null, cancellationToken: cancellationToken);
    }

    public virtual async Task BulkUpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await Context.BulkUpdateAsync(entities, bulkConfig: null, progress: null, type: null, cancellationToken: cancellationToken);
    }

    public virtual async Task<long> CountAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default)
    {
        var count = await Context.Set<TEntity>().LongCountAsync(where, cancellationToken);
        return count;
    }

    public virtual async Task DeleteAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        Context.Set<TEntity>().Remove(entity);
        if (saveChanges)
            await SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(Guid id, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, true, cancellationToken);
        if (entity is null)
            throw new EntityNotFoundException<Guid>(nameof(entity), id);
        Context.Set<TEntity>().Remove(entity);
        if (saveChanges)
            await SaveChangesAsync(cancellationToken);
    }


    public virtual async Task DeleteRangeAsync(IEnumerable<TEntity> entities, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        Context.Set<TEntity>().RemoveRange(entities);
        if (saveChanges)
            await SaveChangesAsync(cancellationToken);
    }


    public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default)
    {
        var any = await Context.Set<TEntity>().AnyAsync(where, cancellationToken);
        return any;
    }


    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var entities = await Context.Set<TEntity>()
            .AsNoTrackingIf(asNoTracking)
            .ToListAsync(cancellationToken);
        return entities;
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> where,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? includes = null,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<TEntity>().AsQueryable();
        
        if (asNoTracking)
            query = query.AsNoTracking();

        query = query.Where(where);

        if (includes != null)
            query = includes(query);

        var entities = await query.ToListAsync(cancellationToken);

        return entities;
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var entity = await Context.Set<TEntity>()
            .AsNoTrackingIf(asNoTracking)
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
        return entity;
    }

  

    public virtual async Task<TEntity?> GetFirstAsync(
        Expression<Func<TEntity, bool>> where,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? includes = null,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<TEntity>().AsQueryable();

        if (asNoTracking)
            query = query.AsNoTracking();

        query = query.Where(where);

        if (includes != null)
            query = includes(query);

        var entity = await query.FirstOrDefaultAsync(cancellationToken);

        return entity;
    }

    public virtual async Task<TEntity?> GetLastAsync(
        Expression<Func<TEntity, bool>> where,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? includes = null,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<TEntity>().AsQueryable();

        if (asNoTracking)
            query = query.AsNoTracking();

        query = query.Where(where);

        if (includes != null)
            query = includes(query);

        var entity = await query.LastOrDefaultAsync(cancellationToken);

        return entity;
    }

    public virtual async Task<TEntity?> GetSingleAsync(
        Expression<Func<TEntity, bool>> where,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? includes = null,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<TEntity>().AsQueryable();

        if (asNoTracking)
            query = query.AsNoTracking();

        query = query.Where(where);

        if (includes != null)
            query = includes(query);

        var entity = await query.SingleOrDefaultAsync(cancellationToken);

        return entity;
    }



    public IQueryable<TEntity> Query()
       => Context.Set<TEntity>().AsNoTracking();

    public IQueryable<TEntity> QueryTracking()
       => Context.Set<TEntity>().AsTracking();


    public virtual async Task<TEntity> UpdateAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        Context.Set<TEntity>().Update(entity);
        if (saveChanges)
            await SaveChangesAsync(cancellationToken);
        return entity;
    }
}
