using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PWC.Challenge.Application.Dtos.Profile;
using PWC.Challenge.Application.Features.Profile.Commands;
using PWC.Challenge.Application.Features.Profile.Queries;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Common;

namespace PWC.Challenge.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBaseRepository<Customer> _customerRepository;

        public ProfileController(ISender sender, UserManager<ApplicationUser> userManager, IBaseRepository<Customer> customerRepository)
        {
            _sender = sender;
            _userManager = userManager;
            _customerRepository = customerRepository;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }

            var userId = Guid.Parse(userIdString);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);
            var customerIdString = User.FindFirstValue("customerId");
            Guid? customerId = null;
            if (!string.IsNullOrEmpty(customerIdString))
            {
                customerId = Guid.Parse(customerIdString);
            }

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null)
            {
                return NotFound(new
                {
                    status = 404,
                    title = "User not found",
                    detail = $"No user was found for ID {userId}"
                });
            }

            Customer? customer = null;
            if (customerId.HasValue)
            {
                customer = await _customerRepository.GetByIdAsync(customerId.Value);
            }

            return Ok(new ProfileDto
            {
                UserId = userId,
                Email = userEmail,
                Role = role,
                Customer = customer != null ? new ProfileCustomerDto
                {
                    Id = customer.Id,
                    FullName = customer.FullName,
                    Address = customer.Address
                } : null
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var command = new UpdateProfileCommand(Guid.Parse(userId), request.FullName, request.Address);
            var updatedProfile = await _sender.Send(command);

            if (updatedProfile == null)
            {
                return NotFound(new
                {
                    status = 404,
                    title = "Profile not found",
                    detail = $"No profile was found for user ID {userId} to update."
                });
            }

            return Ok(updatedProfile);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetProfileByCustomerId(Guid customerId)
        {
            var query = new GetProfileByCustomerIdQuery(customerId);
            var profile = await _sender.Send(query);

            if (profile == null)
            {
                return NotFound(new
                {
                    status = 404,
                    title = "Profile not found",
                    detail = $"No profile was found for customer ID {customerId}"
                });
            }

            return Ok(profile);
        }
    }
}
