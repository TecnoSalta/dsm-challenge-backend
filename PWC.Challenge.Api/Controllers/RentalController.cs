using MediatR;
using Microsoft.AspNetCore.Mvc;
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Application.Features.Commands.Rentals.UpdateReservation;

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
    [ProducesResponseType(typeof(UpdatedReservationDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateReservation(
        [FromRoute] Guid id,
        [FromBody] UpdateReservationDto dto,
        CancellationToken ct)
    {
        var command = new UpdateReservationCommand(id, dto);
        var response = await sender.Send(command, ct);
        return Ok(response);
    }
}