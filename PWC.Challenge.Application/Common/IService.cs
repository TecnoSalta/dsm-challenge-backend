using PWC.Challenge.Application.Common.Entities;
using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Application.Common;

public interface IService<TEntity, TEntityDto>
    where TEntity : IEntity
    where TEntityDto : IEntityDto
   
{
    IBaseRepository<TEntity> Repository { get; } // TODO: Comentar esta línea si no quieres exponer la propiedad Repository en el service.

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
