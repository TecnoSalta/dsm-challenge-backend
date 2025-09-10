using Microsoft.AspNetCore.Mvc;
using PWC.Challenge.Application.Common;
using PWC.Challenge.Application.Common.Entities;
using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Api.Common.Controllers.Traditional;

/// <summary>
/// Controlador de escritura de entidad
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TEntityDto"></typeparam>
/// <typeparam name="TStrongTypeId"></typeparam>
/// <typeparam name="TPrimitiveId"></typeparam>
/// <typeparam name="TQueryFilter"></typeparam>
[ApiController]
public abstract class BaseWriteController<TEntity, TEntityDto> : BaseReadController<TEntity, TEntityDto>, IBaseWriteController<TEntity, TEntityDto>
    where TEntity : Entity
    where TEntityDto : EntityDto
{
    public BaseWriteController(
        ILogger<BaseWriteController<TEntity, TEntityDto>> logger,
        IBaseService<TEntity, TEntityDto> service) : base(logger, service)
    {
    }

    [HttpPost]
    public virtual async Task<IActionResult> CreateAsync(TEntityDto entity)
    {
        var result = await this.service.AddAsync(entity, true, CancellationToken.None);
        return Ok(result);
    }

    [HttpPut]
    public virtual async Task<IActionResult> UpdateAsync(TEntityDto entity)
    {
        var result = await this.service.UpdateAsync(entity, true, CancellationToken.None);
        return Ok(result);
    }


}
