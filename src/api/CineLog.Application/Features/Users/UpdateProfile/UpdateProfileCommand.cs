using MediatR;

namespace CineLog.Application.Features.Users.UpdateProfile;

public record UpdateProfileCommand(string? Bio, string? AvatarUrl) : IRequest<UserProfileResponse>;
