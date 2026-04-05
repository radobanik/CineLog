using CineLog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineLog.Infrastructure.Data.Configurations;

public class WatchlistConfiguration : IEntityTypeConfiguration<Watchlist>
{
    public void Configure(EntityTypeBuilder<Watchlist> builder)
    {
        builder.ToTable("watchlists");
        builder.HasKey(w => w.Id);
        builder.Property(w => w.UserId).IsRequired();
        builder.Property(w => w.Name).HasMaxLength(100).IsRequired();
        builder.Property(w => w.CreatedAt).IsRequired();

        builder.HasMany(w => w.Items)
            .WithOne()
            .HasForeignKey(i => i.WatchlistId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class WatchlistItemConfiguration : IEntityTypeConfiguration<WatchlistItem>
{
    public void Configure(EntityTypeBuilder<WatchlistItem> builder)
    {
        builder.ToTable("watchlist_items");
        builder.HasKey(i => new { i.WatchlistId, i.MovieId });
        builder.Property(i => i.AddedAt).IsRequired();
    }
}
