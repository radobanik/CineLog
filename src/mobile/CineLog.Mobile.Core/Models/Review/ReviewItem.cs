using System;
using System.Collections.Generic;
using System.Text;

namespace CineLog.Mobile.Core.Models.Review
{
    public sealed class ReviewItem
    {
        public Guid ReviewId { get; init; }
        public Guid? MovieId { get; init; }

        public string Username { get; init; } = string.Empty;
        public string? AvatarUrl { get; init; }

        public string MovieTitle { get; init; } = string.Empty;
        public string? PosterPath { get; init; }

        public double? Rating { get; init; }
        public string? ReviewText { get; init; }
        public bool ContainsSpoilers { get; init; }
        public int LikesCount { get; init; }
        public DateTimeOffset? CreatedAt { get; init; }

        public string UserInitial =>
            string.IsNullOrWhiteSpace(Username) ? "?" : Username[..1].ToUpperInvariant();

        public string RatingText =>
            Rating.HasValue ? $"{Rating.Value:0.0}/5" : "-";

        public string ReviewPreview =>
            string.IsNullOrWhiteSpace(ReviewText) ? "No review text added." : ReviewText!;

        public string MetaText
        {
            get
            {
                var createdAtText = CreatedAt?.ToLocalTime().ToString("d MMM yyyy");
                var likesText = LikesCount == 1 ? "1 like" : $"{LikesCount} likes";

                return string.IsNullOrWhiteSpace(createdAtText)
                    ? likesText
                    : $"{createdAtText} • {likesText}";
            }
        }

        public string ActivityText => $"{Username} reviewed";

        public string SpoilerText => ContainsSpoilers ? "Contains spoilers" : string.Empty;
    }
}
