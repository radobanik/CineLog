using CineLog.Domain.Enums;

namespace CineLog.Application.Features.Movies.GetMovieDetail;

public record MovieDetailResponse(
    Guid Id,
    int IdTmdb,
    MovieType Type,
    string Title,
    string? Overview,
    string? PosterPath,
    string? BackdropPath,
    DateOnly? ReleaseDate,
    int? RuntimeMinutes,
    decimal AverageRating,
    int RatingsCount,
    List<GenreResponse> Genres,
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

public record GenreResponse(Guid Id, string Name);

public record CastMemberResponse(
    Guid PersonId,
    string Name,
    string? Character,
    int Order,
    string? ProfilePath);

public record CrewMemberResponse(
    Guid PersonId,
    string Name,
    string? Department,
    string? Job,
    string? ProfilePath);

public record ProductionCompanyResponse(
    Guid Id,
    string Name,
    string? LogoPath,
    string? OriginCountry);
