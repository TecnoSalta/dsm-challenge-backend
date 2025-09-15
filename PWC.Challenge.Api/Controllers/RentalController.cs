using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Added
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Application.Features.Rentals.Commands.CompleteRental;
using PWC.Challenge.Application.Features.Rentals.Commands.CancelRental;
using PWC.Challenge.Application.Features.Rentals.Commands.UpdateRental;
using System.ComponentModel.DataAnnotations;
using PWC.Challenge.Application.Features.Rentals.Commands.CreateRental; // Added for CreateRental

namespace PWC.Challenge.Api.Controllers;

[Authorize] // Added
[Route("api/[controller]")]
[ApiController]
public class RentalsController(
    ILogger<RentalsController> logger,
    ISender sender) : ControllerBase
{
    private readonly ILogger<RentalsController> _logger = logger;
    private readonly ISender _sender = sender;

    /// <summary>
    /// CU-01 – Registrar una nueva reserva
    /// </summary>
    [Authorize(Roles = "Admin,Customer")]
    [HttpPost]
    [ProducesResponseType(typeof(CreatedRentalDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CreateRental(
        [FromBody] CreateRentalDto dto,
        CancellationToken ct)
    {
        _logger.LogInformation("Iniciando registro de nueva reserva para Cliente ID: {CustomerId}, Carro ID: {CarId}", dto.CustomerId, dto.CarId);

        try
        {
            var command = new CreateRentalCommand(dto.CustomerId, dto.CarId, dto.StartDate, dto.EndDate);
            var response = await _sender.Send(command, ct);

            _logger.LogInformation("Reserva con ID: {RentalId} registrada exitosamente", response.Id);
            return CreatedAtAction(nameof(GetRentalById), new { id = response.Id }, response); // Assuming a GetRentalById exists or will exist
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al registrar reserva para Cliente ID: {CustomerId}, Carro ID: {CarId}", dto.CustomerId, dto.CarId);
            return BadRequest(ex.Message);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Recurso no encontrado al registrar reserva para Cliente ID: {CustomerId}, Carro ID: {CarId}", dto.CustomerId, dto.CarId);
            return NotFound(ex.Message);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Regla de negocio violada al registrar reserva para Cliente ID: {CustomerId}, Carro ID: {CarId}", dto.CustomerId, dto.CarId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al registrar reserva para Cliente ID: {CustomerId}, Carro ID: {CarId}", dto.CustomerId, dto.CarId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// CU-04 – Modificar reserva activa
    /// </summary>
    [Authorize(Roles = "Admin,Customer")] // Added
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UpdatedRentalDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateRental(
        [FromRoute] Guid id,
        [FromBody] UpdateRentalDto dto,
        CancellationToken ct)
    {
        _logger.LogInformation("Iniciando actualización de reserva con ID: {RentalId}", id);

        try
        {
            var command = new UpdateRentalCommand(id, dto);
            var response = await _sender.Send(command, ct);

            _logger.LogInformation("Reserva con ID: {RentalId} actualizada exitosamente", id);
           
            return Ok(response);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al actualizar reserva con ID: {RentalId}", id);
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Reserva no encontrada con ID: {RentalId}", id);
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Operación inválida al actualizar reserva con ID: {RentalId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al actualizar reserva con ID: {RentalId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// CU-05 – Cancelar reserva activa
    /// </summary>
    [Authorize(Roles = "Admin,Customer")] // Added
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CancelRental(
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        _logger.LogInformation("Iniciando cancelación de reserva con ID: {RentalId}", id);

        try
        {
            var command = new CancelRentalCommand(id);
            await _sender.Send(command, ct);

            _logger.LogInformation("Reserva con ID: {RentalId} cancelada exitosamente", id);
            return NoContent();
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al cancelar reserva con ID: {RentalId}", id);
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Reserva no encontrada con ID: {RentalId}", id);
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Operación inválida al cancelar reserva con ID: {RentalId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al cancelar reserva con ID: {RentalId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// CU-06 – Completar una reserva activa
    /// </summary>
    [Authorize(Roles = "Admin,Customer")] // Added
    [HttpPost("{id}/complete")]
    [ProducesResponseType(typeof(CompletedRentalDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CompleteRental(
        [FromRoute] Guid id,
        [FromBody] CompleteRentalDto dto,
        CancellationToken ct)
    {
        _logger.LogInformation("Iniciando completado de reserva con ID: {RentalId}", id);

        try
        {
            if (id != dto.RentalId)
            {
                _logger.LogWarning("ID de URL ({UrlId}) no coincide con ID del cuerpo ({BodyId})", id, dto.RentalId);
                return BadRequest("El id de la URL no coincide con el cuerpo.");
            }

            var command = new CompleteRentalCommand(dto.RentalId);
            var response = await _sender.Send(command, ct);

            _logger.LogInformation("Reserva con ID: {RentalId} completada exitosamente", id);
            return Ok(response);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al completar reserva con ID: {RentalId}", id);
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Reserva no encontrada con ID: {RentalId}", id);
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Operación inválida al completar reserva con ID: {RentalId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al completar reserva con ID: {RentalId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}