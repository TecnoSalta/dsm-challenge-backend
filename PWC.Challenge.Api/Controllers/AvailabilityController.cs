using MediatR;
using Microsoft.AspNetCore.Mvc;
using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Application.Features.Cars.Queries.GetAvailableCars;

namespace PWC.Challenge.Api.Controllers;

[Route("api/[controller]")]
public class AvailabilityController(
    ILogger<AvailabilityController> logger,
    ISender sender) : ControllerBase
{

    private readonly ISender sender = sender;

    /// <summary>
    /// Consultar coches disponibles para un rango de fechas
    /// </summary>
    [HttpGet("disponibilidad")]
    [ProducesResponseType(typeof(IReadOnlyList<AvailableCarDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAvailableCars(
        [FromQuery] AvailabilityQueryDto filter,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetAvailableCarsQuery(filter), ct);
        return Ok(result);
    }
}
