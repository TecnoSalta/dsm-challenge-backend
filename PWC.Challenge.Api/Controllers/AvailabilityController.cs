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
    [ProducesResponseType(typeof(IReadOnlyList<AvailableCarDto>), 200)]
    public async Task<IActionResult> GetAvailableCars(
        [FromQuery] AvailabilityQueryDto filter,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetAvailableCarsQuery(filter), ct);
        return Ok(result);
    }
}
