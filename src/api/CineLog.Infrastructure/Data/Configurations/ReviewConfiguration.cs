using CineLog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineLog.Infrastructure.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.UserId).IsRequired();
        builder.Property(r => r.MovieId).IsRequired();

        builder.HasIndex(r => new { r.UserId, r.MovieId }).IsUnique();

        builder.OwnsOne(r => r.Rating, rating =>
        {
            rating.Property(r => r.Value)
                .HasColumnName("rating")
                .HasPrecision(4, 2)
                .IsRequired();
        });

        builder.Property(r => r.ReviewText);
        builder.Property(r => r.ContainsSpoilers).IsRequired();
        builder.Property(r => r.WatchedOn);
        builder.Property(r => r.IsLiked).IsRequired();
        builder.Property(r => r.LikesCount).IsRequired();
        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.UpdatedAt);

        builder.Ignore(r => r.DomainEvents);

        builder.HasMany(r => r.Reactions)
            .WithOne()
            .HasForeignKey(rr => rr.ReviewId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
