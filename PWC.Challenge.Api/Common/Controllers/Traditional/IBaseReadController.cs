using Microsoft.AspNetCore.Mvc;
using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Api.Common.Controllers.Traditional;

public interface IBaseReadController<TEntity>
    where TEntity : IEntity
{
    Task<IActionResult> GetAllAsync();
    Task<IActionResult> GetByIdAsync(Guid id);
}
