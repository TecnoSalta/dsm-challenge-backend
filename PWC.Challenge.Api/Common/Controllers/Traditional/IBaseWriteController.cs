using Microsoft.AspNetCore.Mvc;
using PWC.Challenge.Application.Common.Entities;
using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Api.Common.Controllers.Traditional;

public interface IBaseWriteController<TEntity, TEntityDto> : IBaseReadController<TEntity>
    where TEntity : IEntity
    where TEntityDto : IEntityDto
{
    Task<IActionResult> CreateAsync(TEntityDto entity);
    Task<IActionResult> UpdateAsync(TEntityDto entity);
}
