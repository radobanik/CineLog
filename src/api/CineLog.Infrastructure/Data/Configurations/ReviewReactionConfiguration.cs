using CineLog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineLog.Infrastructure.Data.Configurations;

public class ReviewReactionConfiguration : IEntityTypeConfiguration<ReviewReaction>
{
    public void Configure(EntityTypeBuilder<ReviewReaction> builder)
    {
        builder.ToTable("review_reactions");

        builder.HasKey(rr => rr.Id);

        builder.Property(rr => rr.ReviewId).IsRequired();
        builder.Property(rr => rr.UserId).IsRequired();
        builder.Property(rr => rr.Type).IsRequired();
        builder.Property(rr => rr.CreatedAt).IsRequired();

        builder.HasIndex(rr => new { rr.ReviewId, rr.UserId, rr.Type }).IsUnique();
    }
}
