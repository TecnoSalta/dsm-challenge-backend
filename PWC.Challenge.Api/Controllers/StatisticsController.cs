using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Application.Features.Statistics.Queries.GetMostRentedCarTypes;

namespace PWC.Challenge.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StatisticsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("MostRentedCarTypes")]
    [ProducesResponseType(typeof(List<CarTypeRentalCountDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMostRentedCarTypesAsync(DateOnly? startDate = null, DateOnly? endDate = null)
    {
        var results = await _mediator.Send(new GetMostRentedCarTypeQuery(startDate, endDate));
        return Ok(results);
    }
}