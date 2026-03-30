using MediatR;

namespace CineLog.Application.Features.Users.GetProfile;

public record GetProfileQuery(Guid UserId) : IRequest<UserProfileResponse>;
