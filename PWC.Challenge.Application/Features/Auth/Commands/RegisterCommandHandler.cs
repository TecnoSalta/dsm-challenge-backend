using MediatR;
using Microsoft.AspNetCore.Identity;
using PWC.Challenge.Application.Dtos.Auth;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Domain.Services;
using PWC.Challenge.Common.Exceptions;

namespace PWC.Challenge.Application.Features.Auth.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApplicationUser>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RegisterCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ApplicationUser> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.RegisterRequest.Email);
        if (existingUser != null)
        {
            throw new BadRequestException("User with this email already exists.");
        }

        var user = new ApplicationUser
        {
            Email = request.RegisterRequest.Email,
            UserName = request.RegisterRequest.Email,
            FirstName = request.RegisterRequest.FullName, // Use FullName
            LastName = request.RegisterRequest.Address // Use Address for LastName (temporary, or add a new field)
        };

        var result = await _userManager.CreateAsync(user, request.RegisterRequest.Password);
        if (!result.Succeeded)
        {
            throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // Assign default role, e.g., "Customer"
        await _userManager.AddToRoleAsync(user, "Customer");

        return user;
    }
}
