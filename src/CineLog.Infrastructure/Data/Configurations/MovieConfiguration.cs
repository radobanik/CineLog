using System.Text.Json;
using CineLog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CineLog.Infrastructure.Data.Configurations;

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    private static readonly JsonSerializerOptions JsonOptions = new();

    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.ToTable("movies");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.TmdbId).IsRequired();
        builder.HasIndex(m => m.TmdbId).IsUnique();

        builder.Property(m => m.Title).IsRequired();
        builder.Property(m => m.Overview);
        builder.Property(m => m.PosterPath);
        builder.Property(m => m.BackdropPath);
        builder.Property(m => m.ReleaseDate);
        builder.Property(m => m.RuntimeMinutes);
        builder.Property(m => m.Type).IsRequired();

        builder.Property(m => m.AverageRating)
            .HasPrecision(4, 2);

        builder.Property(m => m.RatingsCount);

        // Genres stored as jsonb — backed by private List<string> _genres field
        builder.Property<List<string>>("_genres")
            .HasColumnName("genres")
            .HasColumnType("jsonb")
            .HasConversion(
                new ValueConverter<List<string>, string>(
                    v => JsonSerializer.Serialize(v, JsonOptions),
                    v => JsonSerializer.Deserialize<List<string>>(v, JsonOptions) ?? new List<string>()
                )
            );
    }
}
