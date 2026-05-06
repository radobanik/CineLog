using System;
using System.Collections.Generic;
using System.Text;

namespace CineLog.Mobile.Core.Models.Search
{
    public sealed class UserSearchItem
    {
        public Guid Id { get; init; }
        public string Username { get; init; } = string.Empty;
        public string? AvatarUrl { get; init; }
        public int ReviewCount { get; init; }
        public bool IsFollowing { get; init; }
    }
}
