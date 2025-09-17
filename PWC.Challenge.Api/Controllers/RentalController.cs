using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PWC.Challenge.Application.Dtos.Rentals;
using PWC.Challenge.Application.Features.Rentals.Commands.CompleteRental;
using PWC.Challenge.Application.Features.Rentals.Commands.CancelRental;
using PWC.Challenge.Application.Features.Rentals.Commands.UpdateRental;
using System.ComponentModel.DataAnnotations;
using PWC.Challenge.Application.Features.Rentals.Commands.CreateRental;
using PWC.Challenge.Application.Exceptions;
using PWC.Challenge.Common.Exceptions;
using PWC.Challenge.Application.Features.Rentals.Queries.GetRentalById;
using PWC.Challenge.Application.Features.Rentals.Queries.GetRentals;
using PWC.Challenge.Application.Services;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Exceptions;

namespace PWC.Challenge.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class RentalsController(
    ILogger<RentalsController> logger,
    ISender sender,
    IRentalService rentalService) : ControllerBase
{
    private readonly ILogger<RentalsController> _logger = logger;
    private readonly ISender _sender = sender;
    private readonly IRentalService _rentalService = rentalService;

    /// <summary>
    /// Registrar un nuevo alquiler
    /// </summary>
    [Authorize(Roles = "Admin,Customer")]
    [HttpPost]
    [ProducesResponseType(typeof(Rental), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RegisterRental(
        [FromBody] CreateRentalRequestDto request,
        CancellationToken ct)
    {
        _logger.LogInformation("Iniciando registro de nuevo alquiler para Cliente ID: {CustomerId}, Carro ID: {CarId}", request.CustomerId, request.CarId);

        try
        {
            var rental = await _rentalService.RegisterRentalAsync(request);

            _logger.LogInformation("Alquiler con ID: {RentalId} registrado exitosamente", rental.Id);
            return CreatedAtAction(
                nameof(GetRentalById),
                new { id = rental.Id },
                rental
            );

        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al registrar reserva para Cliente ID: {CustomerId}, Carro ID: {CarId}", request.CustomerId, request.CarId);
            return BadRequest(ex.Message);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Recurso no encontrado al registrar reserva para Cliente ID: {CustomerId}, Carro ID: {CarId}", request.CustomerId, request.CarId);
            return NotFound(ex.Message);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Regla de negocio violada al registrar reserva para Cliente ID: {CustomerId}, Carro ID: {CarId}", request.CustomerId, request.CarId);
            return BadRequest(ex.Message);
        }
        catch (CarNotAvailableException ex)
        {
            _logger.LogWarning(ex, "Auto no disponible para el alquiler: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (OverlappingRentalException ex)
        {
            _logger.LogWarning(ex, "Período de alquiler se solapa: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogWarning(ex, "Argumento nulo al registrar alquiler: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al registrar alquiler: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, "Error interno del servidor");
        }
    }

    /// <summary>
    /// CU-04 – Modificar reserva activa
    /// </summary>
    [Authorize(Roles = "Admin,Customer")]
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
    [Authorize(Roles = "Admin,Customer")]
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
    [Authorize(Roles = "Admin,Customer")]
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
            var command = new CompleteRentalCommand(id, dto.RentalId);
            var response = await _sender.Send(command, ct);

            _logger.LogInformation("Reserva con ID: {RentalId} completada exitosamente", id);
            return Ok(response); // Cambiado de Created a Ok ya que no se está creando un nuevo recurso
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al completar reserva con ID: {RentalId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// CU-XX – Obtener detalles de una reserva por ID
    /// </summary>
    [Authorize(Roles = "Admin,Customer")]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RentalDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetRentalById(
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        _logger.LogInformation("Iniciando consulta de reserva con ID: {RentalId}", id);

        try
        {
            var query = new GetRentalByIdQuery(id);
            var response = await _sender.Send(query, ct);

            _logger.LogInformation("Reserva con ID: {RentalId} consultada exitosamente", id);
            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Reserva no encontrada con ID: {RentalId}", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al consultar reserva con ID: {RentalId}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// CU-XX – Obtener todas las reservas
    /// </summary>
    [Authorize(Roles = "Admin,Customer")]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<RentalDto>), 200)]
    public async Task<IActionResult> GetRentals(CancellationToken ct)
    {
        _logger.LogInformation("Iniciando consulta de todas las reservas");

        var query = new GetRentalsQuery();
        var response = await _sender.Send(query, ct);

        _logger.LogInformation("Consulta de todas las reservas completada exitosamente");
        return Ok(response);
    }
}