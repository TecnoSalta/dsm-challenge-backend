using MediatR;
using Microsoft.AspNetCore.Mvc;
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Application.Features.Rentals.Commands.CancelRental;
using PWC.Challenge.Application.Features.Rentals.Commands.UpdateRental;

namespace PWC.Challenge.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RentalsController : ControllerBase
{

    private readonly ISender sender;

    public RentalsController(
        ILogger<RentalsController> logger,
        ISender sender)
    {
        this.sender = sender;
    }
    /// <summary>
    //CU-04 – Modificar reserva activa
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UpdatedRentalDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateRental(
        [FromRoute] Guid id,
        [FromBody] UpdateRentalDto dto,
        CancellationToken ct)
    {
        var command = new UpdateRentalCommand(id, dto);
        var response = await sender.Send(command, ct);
        return Ok(response);
    }


    /// <summary>
    /// CU-05 – Cancelar reserva activa
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CancelRental(
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var command = new CancelRentalCommand(id);
        await sender.Send(command, ct);
        return NoContent();
    }
}