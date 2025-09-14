using MediatR;
using Microsoft.AspNetCore.Mvc;
using PWC.Challenge.Application.Dtos.Auth;
using PWC.Challenge.Application.Features.Auth.Commands;

namespace PWC.Challenge.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        var command = new RegisterCommand(request);
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var command = new LoginCommand(request);
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }
}
