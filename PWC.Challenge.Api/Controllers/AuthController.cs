using MediatR;
using Microsoft.AspNetCore.Mvc;
using PWC.Challenge.Application.Dtos.Auth;
using PWC.Challenge.Application.Features.Auth.Commands;
using Microsoft.AspNetCore.Identity;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Application.Features.Auth.Commands.RegisterCustomer;
using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IBaseRepository<Customer> _customerRepository;

    public AuthController(IMediator mediator, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IBaseRepository<Customer> customerRepository)
    {
        _mediator = mediator;
        _userManager = userManager;
        _signInManager = signInManager;
        _customerRepository = customerRepository;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        var user = new ApplicationUser 
        {
            UserName = request.Email, 
            Email = request.Email,
            FirstName = request.FullName, // Set FirstName
            LastName = request.Address // Set LastName
        };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        await _userManager.AddToRoleAsync(user, "Customer"); // Assign Customer role

        var registerCustomerCommand = new RegisterCustomerCommand(
            request.FullName,
            request.Dni,
            request.Address,
            request.Email,
            user.Id
        );
        var customerResponse = await _mediator.Send(registerCustomerCommand);

        var roles = await _userManager.GetRolesAsync(user);
        var tokenResponse = await _mediator.Send(new LoginCommand(user.Email, user.Id, roles.ToList(), customerResponse.CustomerId));

        return CreatedAtAction(nameof(Register), new RegisterResponseDto
        (
            user.Id,
            customerResponse.CustomerId,
            roles.FirstOrDefault() ?? ""
        ));
    }

        [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return Unauthorized();

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded) return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);
        var customer = await _customerRepository.GetSingleAsync(c => c.UserId == user.Id);

        var tokenResponse = await _mediator.Send(new LoginCommand(request.Email, user.Id, roles.ToList(), customer?.Id));

        return Ok(tokenResponse);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }
}