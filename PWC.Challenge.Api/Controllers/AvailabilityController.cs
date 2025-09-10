using MediatR;
using Microsoft.AspNetCore.Mvc;
using PWC.Challenge.Application.Common;
using PWC.Challenge.Application.Dtos;
using PWC.Challenge.Application.Features.Querys.GetAvailableCars;
using PWC.Challenge.Domain.Entities;

namespace PWC.Challenge.Api.Controllers;

// PWC.Challenge.Api\Controllers\AvailabilityController.cs
[Route("api/[controller]")]
public class AvailabilityController(
    ILogger<AvailabilityController> logger,
    ISender sender) : ControllerBase
{

    private readonly ISender sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAvailableCars(
        [FromQuery] AvailabilityQueryDto filter,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetAvailableCarsQuery(filter), ct);
        return Ok(result);
    }
}


