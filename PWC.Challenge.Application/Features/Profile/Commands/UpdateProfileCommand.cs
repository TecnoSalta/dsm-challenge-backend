using MediatR;
using PWC.Challenge.Application.Dtos.Profile;

namespace PWC.Challenge.Application.Features.Profile.Commands;

public record UpdateProfileCommand(Guid UserId, string FullName, string Address) : IRequest<ProfileDto>;
