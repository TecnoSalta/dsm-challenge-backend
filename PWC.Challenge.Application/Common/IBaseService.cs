using PWC.Challenge.Application.Common.Entities;
using PWC.Challenge.Domain.Common;
using System.Linq.Expressions;

namespace PWC.Challenge.Application.Common;

public interface IBaseService<TEntity, TEntityDto> : IService<TEntity, TEntityDto>
    where TEntity : IEntity
    where TEntityDto : IEntityDto
{
    Task<TEntityDto> AddAsync(TEntityDto entityDto, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task<TEntity> AddAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<TEntityDto> entityDtos, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task BulkAddAsync(IEnumerable<TEntityDto> entityDtos, CancellationToken cancellationToken = default);
    Task BulkDeleteAsync(IEnumerable<TEntityDto> entityDtos, CancellationToken cancellationToken = default);
    Task BulkUpdateAsync(IEnumerable<TEntityDto> entityDtos, CancellationToken cancellationToken = default);
    Task<long> CountAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntityDto entity, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<TEntityDto> entityDtos, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntityDto>> GetAllAsync(bool asNoTracking = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntityDto>> GetAllAsync(Expression<Func<TEntity, bool>> where, Func<IQueryable<TEntity>, IQueryable<TEntity>>? includes, bool asNoTracking = false, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<TEntityDto> UpdateAsync(TEntityDto entityDto, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task<TEntity> UpdateAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, bool saveChanges = true, CancellationToken cancellationToken = default);
    Task<TEntity?> FindByIdAsync(Guid id, bool asNoTracking = false, CancellationToken cancellationToken = default);
    Task<TEntityDto?> GetByIdAsync(Guid id, bool asNoTracking = false, CancellationToken cancellationToken = default);

}
