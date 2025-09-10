using System.Linq.Expressions;

namespace PWC.Challenge.Domain.Common;

public interface IBaseRepository<TEntity> : IRepository
    where TEntity : IEntity
{
    IQueryable<TEntity> Query();
    Task<TEntity> AddAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<TEntity> entities, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task BulkAddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task BulkDeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task BulkSaveChangesAsync(CancellationToken cancellationToken = default);
    Task BulkUpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task<long> CountAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> where, Func<IQueryable<TEntity>, IQueryable<TEntity>>? includes = null, bool asNoTracking = false, CancellationToken cancellationToken = default);
    Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>> where, Func<IQueryable<TEntity>, IQueryable<TEntity>>? includes = null, bool asNoTracking = false, CancellationToken cancellationToken = default);
    Task<TEntity?> GetLastAsync(Expression<Func<TEntity, bool>> where, Func<IQueryable<TEntity>, IQueryable<TEntity>>? includes = null, bool asNoTracking = false, CancellationToken cancellationToken = default);
    Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> where, Func<IQueryable<TEntity>, IQueryable<TEntity>>? includes = null, bool asNoTracking = false, CancellationToken cancellationToken = default);
    Task<TEntity> UpdateAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(Guid id, bool asNoTracking = false, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, bool saveChanges = true, CancellationToken cancellationToken = default);


}
