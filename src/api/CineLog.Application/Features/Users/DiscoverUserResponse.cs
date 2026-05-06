using System;
using System.Collections.Generic;
using System.Text;

namespace CineLog.Application.Features.Users;

public record DiscoverUserResponse(
    Guid Id,
    string Username,
    string? AvatarUrl,
    int ReviewCount,
    bool IsFollowing);

