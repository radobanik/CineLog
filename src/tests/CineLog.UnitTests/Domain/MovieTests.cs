using CineLog.Domain.Entities;
using CineLog.Domain.Enums;
using FluentAssertions;

namespace CineLog.UnitTests.Domain;

public class MovieTests
{
    [Fact]
    public void Create_SetsPropertiesAndGeneratesId()
    {
        var movie = Movie.Create(12345, "Inception", MovieType.Movie);

        movie.Id.Should().NotBeEmpty();
        movie.IdTmdb.Should().Be(12345);
        movie.Title.Should().Be("Inception");
        movie.Type.Should().Be(MovieType.Movie);
        movie.AverageRating.Should().Be(0);
        movie.RatingsCount.Should().Be(0);
        movie.IsManuallyEdited.Should().BeFalse();
    }

    [Theory]
    [InlineData(MovieType.Movie)]
    [InlineData(MovieType.Series)]
    public void Create_AllMovieTypes_Succeed(MovieType type)
    {
        var movie = Movie.Create(1, "Title", type);
        movie.Type.Should().Be(type);
    }

    [Fact]
    public void Create_TwoMovies_HaveDistinctIds()
    {
        var a = Movie.Create(1, "A", MovieType.Movie);
        var b = Movie.Create(2, "B", MovieType.Movie);

        a.Id.Should().NotBe(b.Id);
    }

    [Fact]
    public void UpdateDetails_SetsAllOptionalFields()
    {
        var movie = Movie.Create(1, "Title", MovieType.Movie);

        movie.UpdateDetails(
            overview: "Some overview",
            posterPath: "/poster.jpg",
            backdropPath: "/backdrop.jpg",
            releaseDate: new DateOnly(2010, 7, 16),
            runtimeMinutes: 148,
            imdbId: "tt1375666",
            originalTitle: "Inception",
            originalLanguage: "en",
            tagline: "Your mind is the scene of the crime.",
            status: "Released",
            budget: 160_000_000,
            revenue: 836_000_000,
            numberOfSeasons: null,
            numberOfEpisodes: null);

        movie.Overview.Should().Be("Some overview");
        movie.PosterPath.Should().Be("/poster.jpg");
        movie.BackdropPath.Should().Be("/backdrop.jpg");
        movie.ReleaseDate.Should().Be(new DateOnly(2010, 7, 16));
        movie.RuntimeMinutes.Should().Be(148);
        movie.ImdbId.Should().Be("tt1375666");
        movie.OriginalTitle.Should().Be("Inception");
        movie.OriginalLanguage.Should().Be("en");
        movie.Tagline.Should().Be("Your mind is the scene of the crime.");
        movie.Status.Should().Be("Released");
        movie.Budget.Should().Be(160_000_000);
        movie.Revenue.Should().Be(836_000_000);
    }

    [Fact]
    public void UpdateDetails_NullValues_ClearsFields()
    {
        var movie = Movie.Create(1, "Title", MovieType.Movie);
        movie.UpdateDetails("overview", "/p.jpg", null, null, null);

        movie.UpdateDetails(null, null, null, null, null);

        movie.Overview.Should().BeNull();
        movie.PosterPath.Should().BeNull();
    }

    [Fact]
    public void UpdateAverageRating_SetsValues()
    {
        var movie = Movie.Create(1, "Title", MovieType.Movie);

        movie.UpdateAverageRating(4.2m, 37);

        movie.AverageRating.Should().Be(4.2m);
        movie.RatingsCount.Should().Be(37);
    }

    [Fact]
    public void UpdateAverageRating_Overwrites_PreviousValues()
    {
        var movie = Movie.Create(1, "Title", MovieType.Movie);
        movie.UpdateAverageRating(3.0m, 10);

        movie.UpdateAverageRating(4.5m, 20);

        movie.AverageRating.Should().Be(4.5m);
        movie.RatingsCount.Should().Be(20);
    }

    [Fact]
    public void UpdatePopularity_SetsValue()
    {
        var movie = Movie.Create(1, "Title", MovieType.Movie);

        movie.UpdatePopularity(123.45);

        movie.Popularity.Should().Be(123.45);
    }

    [Fact]
    public void UpdatePopularity_NullValue_ClearsField()
    {
        var movie = Movie.Create(1, "Title", MovieType.Movie);
        movie.UpdatePopularity(50.0);

        movie.UpdatePopularity(null);

        movie.Popularity.Should().BeNull();
    }

    [Fact]
    public void UpdateTitle_ChangesTitle()
    {
        var movie = Movie.Create(1, "Old Title", MovieType.Movie);

        movie.UpdateTitle("New Title");

        movie.Title.Should().Be("New Title");
    }

    [Fact]
    public void MarkAsManuallyEdited_SetsFlag()
    {
        var movie = Movie.Create(1, "Title", MovieType.Movie);
        movie.IsManuallyEdited.Should().BeFalse();

        movie.MarkAsManuallyEdited();

        movie.IsManuallyEdited.Should().BeTrue();
    }
}
