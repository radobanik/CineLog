using CineLog.Domain.Enums;

namespace CineLog.Application.Features.Movies.GetMovieDetail;

public record MovieDetailResponse(
    Guid Id,
    int TmdbId,
    MovieType Type,
    string Title,
    string? Overview,
    string? PosterPath,
    string? BackdropPath,
    DateOnly? ReleaseDate,
    int? RuntimeMinutes,
    decimal AverageRating,
    int RatingsCount,
    List<string> Genres,
    string? ImdbId,
    string? OriginalTitle,
    string? OriginalLanguage,
    string? Tagline,
    string? Status,
    long? Budget,
    decimal? Revenue,
    double? Popularity,
    int? NumberOfSeasons,
    int? NumberOfEpisodes,
    List<CastMemberResponse> Cast,
    List<CrewMemberResponse> Crew,
    List<ProductionCompanyResponse> ProductionCompanies);

public record CastMemberResponse(
    int PersonId,
    string Name,
    string? Character,
    int Order,
    string? ProfilePath);

public record CrewMemberResponse(
    int PersonId,
    string Name,
    string? Department,
    string? Job,
    string? ProfilePath);

public record ProductionCompanyResponse(
    int Id,
    string Name,
    string? LogoPath,
    string? OriginCountry);
