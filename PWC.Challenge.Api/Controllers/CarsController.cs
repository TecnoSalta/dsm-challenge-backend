using MediatR;
using Microsoft.AspNetCore.Mvc;
using PWC.Challenge.Application.Features.Cars.Queries.GetCarMetadata;

namespace PWC.Challenge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CarsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("metadata")]
    [ProducesResponseType(typeof(List<CarMetadataDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCarMetadata()
    {
        var metadata = await _mediator.Send(new GetCarMetadataQuery());
        return Ok(metadata);
    }
}