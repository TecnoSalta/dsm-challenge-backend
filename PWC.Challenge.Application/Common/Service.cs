using PWC.Challenge.Application.Common.Entities;
using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Application.Common;

public abstract class Service<TEntity, TEntityDto> : IService<TEntity, TEntityDto>
    where TEntity : Entity
    where TEntityDto : EntityDto
{
    //public readonly IEntityRepository<TEntity, TStrongTypeId, TPrimitiveId, TQueryFilter> Repository; // TODO: Descomentar esta línea y comentar la siguiente si no quieres exponer la propiedad Repository en el service.
    public IBaseRepository<TEntity> Repository { get; }

    public Service(IBaseRepository<TEntity> repository)
    {
        Repository = repository;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Repository.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Repository.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Repository.RollbackTransactionAsync(cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var saveChangesResult = await Repository.SaveChangesAsync(cancellationToken);
        return saveChangesResult;
    }
}
