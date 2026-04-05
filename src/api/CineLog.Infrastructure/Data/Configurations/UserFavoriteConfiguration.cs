using CineLog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CineLog.Infrastructure.Data.Configurations;

public class UserFavoriteConfiguration : IEntityTypeConfiguration<UserFavorite>
{
    public void Configure(EntityTypeBuilder<UserFavorite> builder)
    {
        builder.ToTable("user_favorites");

        builder.HasKey(f => new { f.UserId, f.MovieId });

        builder.Property(f => f.AddedAt).IsRequired();
    }
}
