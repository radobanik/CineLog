using CineLog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineLog.Infrastructure.Data.Configurations;

public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.ToTable("activity_logs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.ActorUserId).IsRequired();
        builder.Property(a => a.Type).HasConversion<int>().IsRequired();
        builder.Property(a => a.CreatedAt).IsRequired();

        builder.HasIndex(a => a.ActorUserId);
        builder.HasIndex(a => a.TargetUserId);
        builder.HasIndex(a => a.MovieId);
        builder.HasIndex(a => a.ReviewId);
        builder.HasIndex(a => a.WatchlistId);
        builder.HasIndex(a => a.CreatedAt);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(a => a.ActorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(a => a.TargetUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Movie>()
            .WithMany()
            .HasForeignKey(a => a.MovieId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<Review>()
            .WithMany()
            .HasForeignKey(a => a.ReviewId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<Watchlist>()
            .WithMany()
            .HasForeignKey(a => a.WatchlistId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
