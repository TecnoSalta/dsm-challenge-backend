using PWC.Challenge.Application.Common.Entities;
using PWC.Challenge.Domain.Common;
using System.Linq.Expressions;

namespace PWC.Challenge.Application.Common;

public class BaseService<TEntity, TEntityDto> : Service<TEntity, TEntityDto>, IBaseService<TEntity, TEntityDto>
    where TEntity : Entity
    where TEntityDto : EntityDto
    
{
    public BaseService(IBaseRepository<TEntity> repository) : base(repository)
    {
    }

    public virtual async Task<TEntityDto> AddAsync(TEntityDto entityDto, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        var entity = entityDto.Adapt<TEntity>();
        await Repository.AddAsync(entity, saveChanges, cancellationToken);
        return entityDto;
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        await Repository.AddAsync(entity, saveChanges, cancellationToken);
        return entity;
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntityDto> entityDtos, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        var entities = entityDtos.Adapt<IEnumerable<TEntity>>();
        await Repository.AddRangeAsync(entities, saveChanges, cancellationToken);
    }

    public virtual async Task BulkAddAsync(IEnumerable<TEntityDto> entityDtos, CancellationToken cancellationToken = default)
    {
        var entities = entityDtos.Adapt<IEnumerable<TEntity>>();
        await Repository.BulkAddAsync(entities, cancellationToken);
    }

    public virtual async Task BulkDeleteAsync(IEnumerable<TEntityDto> entityDtos, CancellationToken cancellationToken = default)
    {
        var entities = entityDtos.Adapt<IEnumerable<TEntity>>();
        await Repository.BulkDeleteAsync(entities, cancellationToken);
    }

    public virtual async Task BulkUpdateAsync(IEnumerable<TEntityDto> entityDtos, CancellationToken cancellationToken = default)
    {
        var entities = entityDtos.Adapt<IEnumerable<TEntity>>();
        await Repository.BulkUpdateAsync(entities, cancellationToken);
    }

    public virtual async Task<long> CountAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default)
    {
        var count = await Repository.CountAsync(where, cancellationToken);
        return count;
    }

    public virtual async Task<long> CountAsync(TQueryFilter filter, CancellationToken cancellationToken = default)
    {
        var count = await Repository.CountAsync(filter, cancellationToken);
        return count;
    }

    public virtual async Task DeleteAsync(TEntityDto entityDto, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        var entity = entityDto.Adapt<TEntity>();
        await Repository.DeleteAsync(entity, saveChanges, cancellationToken);
    }

    public virtual async Task DeleteAsync(TPrimitiveId primitiveId, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        await Repository.DeleteAsync(primitiveId, saveChanges, cancellationToken);
    }

    public virtual async Task DeleteAsync(TQueryFilter filter, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        await Repository.DeleteAsync(filter, saveChanges, cancellationToken);
    }

    public virtual async Task DeleteAsync(TStrongTypeId id, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        await Repository.DeleteAsync(id, saveChanges, cancellationToken);
    }

    public virtual async Task DeleteInCascadeAsync(TQueryFilter filter, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        await Repository.DeleteInCascadeAsync(filter, saveChanges, cancellationToken);
    }

    public virtual async Task DeleteRangeAsync(IEnumerable<TEntityDto> entityDtos, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        var entities = entityDtos.Adapt<IEnumerable<TEntity>>();
        await Repository.DeleteRangeAsync(entities, saveChanges, cancellationToken);
    }

    public virtual async Task DeleteRangeAsync(IEnumerable<TPrimitiveId> ids, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        await Repository.DeleteRangeAsync(ids, saveChanges, cancellationToken);
    }

    public virtual async Task DeleteRangeAsync(TQueryFilter filter, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        await Repository.DeleteRangeAsync(filter, saveChanges, cancellationToken);
    }

    public virtual async Task DeleteRangeAsync(IEnumerable<TStrongTypeId> ids, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        await Repository.DeleteRangeAsync(ids, saveChanges, cancellationToken);
    }

    public virtual async Task DeleteRangeInCascadeAsync(TQueryFilter filter, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        await Repository.DeleteRangeInCascadeAsync(filter, saveChanges, cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default)
    {
        var exists = await Repository.ExistsAsync(where, cancellationToken);
        return exists;
    }

    public virtual async Task<bool> ExistsAsync(TQueryFilter filter, CancellationToken cancellationToken = default)
    {
        var exists = await Repository.ExistsAsync(filter, cancellationToken);
        return exists;
    }

    public virtual async Task<TEntity?> FindByIdAsync(TPrimitiveId id, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetByIdAsync(id, asNoTracking, cancellationToken);
        return entity;
    }

    public virtual async Task<TEntity?> FindByIdAsync(TStrongTypeId id, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetByIdAsync(id, asNoTracking, cancellationToken);
        return entity;
    }

    public virtual async Task<IEnumerable<TEntityDto>> GetAllAsync(bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var entities = await Repository.GetAllAsync(asNoTracking, cancellationToken);
        var entityDtos = entities.Adapt<IEnumerable<TEntityDto>>();
        return entityDtos;
    }

    public virtual async Task<IEnumerable<TEntityDto>> GetAllAsync(
        Expression<Func<TEntity, bool>> where,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? includes,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default)
    {
        var entities = await Repository.GetAllAsync(where, includes, asNoTracking, cancellationToken);
        var entityDtos = entities.Adapt<IEnumerable<TEntityDto>>();
        return entityDtos;
    }

    public virtual async Task<IEnumerable<TEntityDto>> GetAllAsync(TQueryFilter filter, CancellationToken cancellationToken = default)
    {
        var entities = await Repository.GetAllAsync(filter, cancellationToken);
        var entityDtos = entities.Adapt<IEnumerable<TEntityDto>>();
        return entityDtos;
    }

    public virtual async Task<TEntityDto?> GetByIdAsync(TPrimitiveId id, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetByIdAsync(id, asNoTracking, cancellationToken);
        var entityDto = entity.Adapt<TEntityDto>();
        return entityDto;
    }

    public virtual async Task<TEntityDto?> GetByIdAsync(TStrongTypeId id, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetByIdAsync(id, asNoTracking, cancellationToken);
        var entityDto = entity.Adapt<TEntityDto>();
        return entityDto;
    }

    public virtual async Task<IEnumerable<TEntityDto>> GetByIdsAsync(
        IEnumerable<TPrimitiveId> ids,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default)
    {
        var entities = await Repository.GetByIdsAsync(ids, asNoTracking, cancellationToken);
        var entityDtos = entities.Adapt<IEnumerable<TEntityDto>>();
        return entityDtos;
    }

    public virtual async Task<IEnumerable<TEntityDto>> GetByIdsAsync(
        IEnumerable<TStrongTypeId> ids,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default)
    {
        var entities = await Repository.GetByIdsAsync(ids, asNoTracking, cancellationToken);
        var entityDtos = entities.Adapt<IEnumerable<TEntityDto>>();
        return entityDtos;
    }

    public virtual async Task<TEntityDto?> GetFirstAsync(TQueryFilter filter, CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetFirstAsync(filter, cancellationToken);
        var entityDto = entity.Adapt<TEntityDto>();
        return entityDto;
    }

    public virtual async Task<TEntityDto?> GetLastAsync(TQueryFilter filter, CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetLastAsync(filter, cancellationToken);
        var entityDto = entity.Adapt<TEntityDto>();
        return entityDto;
    }

    public virtual async Task<PageResult<TEntityDto>> GetPageAsync(TQueryFilter filter, CancellationToken cancellationToken = default)
    {
        var pageResult = await Repository.GetPageAsync(filter, cancellationToken);
        var entityDtos = pageResult.Results.Adapt<IEnumerable<TEntityDto>>();
        return new PageResult<TEntityDto>(pageResult.PageIndex, pageResult.PageSize, pageResult.Count, entityDtos);
    }

    public virtual async Task<TEntityDto?> GetSingleAsync(TQueryFilter filter, CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetSingleAsync(filter, cancellationToken);
        var entityDto = entity.Adapt<TEntityDto>();
        return entityDto;
    }

    public virtual async Task<TEntityDto> UpdateAsync(TEntityDto entityDto, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        var entity = entityDto.Adapt<TEntity>();
        await Repository.UpdateAsync(entity, saveChanges, cancellationToken);
        return entityDto;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        await Repository.UpdateAsync(entity, saveChanges, cancellationToken);
        return entity;
    }
}
