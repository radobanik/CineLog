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

        builder.Property(m => m.ImdbId).HasMaxLength(20);
        builder.Property(m => m.OriginalTitle).HasMaxLength(500);
        builder.Property(m => m.OriginalLanguage).HasMaxLength(10);
        builder.Property(m => m.Tagline).HasMaxLength(500);
        builder.Property(m => m.Status).HasMaxLength(50);
        builder.Property(m => m.Budget);
        builder.Property(m => m.Revenue).HasPrecision(18, 2);
        builder.Property(m => m.Popularity);
        builder.Property(m => m.NumberOfSeasons);
        builder.Property(m => m.NumberOfEpisodes);
        builder.Property(m => m.IsManuallyEdited);

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

        builder.HasMany(m => m.Cast)
            .WithOne(c => c.Movie)
            .HasForeignKey(c => c.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.Crew)
            .WithOne(c => c.Movie)
            .HasForeignKey(c => c.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.ProductionCompanies)
            .WithOne(p => p.Movie)
            .HasForeignKey(p => p.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
