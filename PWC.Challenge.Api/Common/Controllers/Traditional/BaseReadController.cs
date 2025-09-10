using Microsoft.AspNetCore.Mvc;
using PWC.Challenge.Application.Common;
using PWC.Challenge.Application.Common.Entities;
using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Api.Common.Controllers.Traditional;

/// <summary>
/// Controlador de lectura de entidad
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TEntityDto"></typeparam>
/// <typeparam name="TStrongTypeId"></typeparam>
/// <typeparam name="TPrimitiveId"></typeparam>
/// <typeparam name="TQueryFilter"></typeparam>
[ApiController]
public abstract class BaseReadController<TEntity, TEntityDto> : ControllerBase, IBaseReadController<TEntity>
    where TEntity : Entity
    where TEntityDto : EntityDto
{
    protected readonly ILogger<BaseReadController<TEntity, TEntityDto>> logger;
    protected readonly IBaseService<TEntity, TEntityDto> service;

    public BaseReadController(
        ILogger<BaseReadController<TEntity, TEntityDto>> logger,
        IBaseService<TEntity, TEntityDto> service)
    {
        this.logger = logger;
        this.service = service;
    }

    [HttpGet]
    public virtual async Task<IActionResult> GetAllAsync()
    {
        var response = await service.GetAllAsync(true, CancellationToken.None);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public virtual async Task<IActionResult> GetByIdAsync(Guid id)
    {
        return Ok();
    }

}
