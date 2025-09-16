using MediatR;
using PWC.Challenge.Application.Dtos.Profile;

namespace PWC.Challenge.Application.Features.Profile.Queries;

public record GetProfileByUserIdQuery(Guid UserId) : IRequest<ProfileDto>;
