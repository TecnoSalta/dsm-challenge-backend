using MediatR;
using Microsoft.AspNetCore.Mvc;
using PWC.Challenge.Application.Features.Cars.Queries.GetCarMetadata;
using PWC.Challenge.Application.Features.Cars.Queries.GetNextCarServices;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Application.Features.Cars.Queries.GetCars;
using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Application.Services; // Added for ICarService

namespace PWC.Challenge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICarService _carService; // Injected ICarService

    public CarsController(IMediator mediator, ICarService carService) // Modified constructor
    {
        _mediator = mediator;
        _carService = carService; // Assign ICarService
    }

    [HttpGet("metadata")]
    [ProducesResponseType(typeof(List<CarMetadataDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCarMetadata()
    {
        var metadata = await _mediator.Send(new GetCarMetadataQuery());
        return Ok(metadata);
    }

    [HttpGet("available")] // New endpoint
    [ProducesResponseType(typeof(IEnumerable<AvailableCarDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAvailableCars(
        DateOnly startDate,
        DateOnly endDate,
        string? carType,
        string? carModel)
    {
        if (startDate == default || endDate == default)
        {
            return BadRequest("Start date and end date are required.");
        }

        var availableCars = await _carService.GetAvailableCarsAsync(startDate, endDate, carType, carModel);
        return Ok(availableCars);
    }

    [HttpGet("{carId}/availability")] // New endpoint
    [ProducesResponseType(typeof(CarAvailabilityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCarAvailability(
        Guid carId,
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate)
    {
        if (startDate == default || endDate == default)
        {
            return BadRequest("Start date and end date are required.");
        }

        var carAvailability = await _carService.GetCarAvailabilityAsync(carId, startDate, endDate);
        return Ok(carAvailability);
    }

    [HttpGet("NextCarServices")]
    [ProducesResponseType(typeof(IReadOnlyList<Car>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNextCarServices()
    {
        var results = await _mediator.Send(new GetNextCarServicesQuery());
        return Ok(results);
    }

    /// <summary>
    /// Obtener todas los coches con sus servicios
    /// </summary>
    [HttpGet] // This will map to /api/Cars
    [ProducesResponseType(typeof(IReadOnlyList<CarDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCars()
    {
        var query = new GetCarsQuery();
        var response = await _mediator.Send(query);
        return Ok(response);
    }
}
