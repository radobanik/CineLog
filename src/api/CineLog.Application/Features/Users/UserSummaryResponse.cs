namespace CineLog.Application.Features.Users;

public record UserSummaryResponse(Guid Id, string Username, string? AvatarUrl);
