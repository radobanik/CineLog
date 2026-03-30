using CineLog.TmdbSync.Data;
using CineLog.TmdbSync.Infrastructure;
using CineLog.TmdbSync.Sync;
using DM.MovieApi;
using DM.MovieApi.MovieDb.Discover;
using DM.MovieApi.MovieDb.Movies;
using DM.MovieApi.MovieDb.People;
using DM.MovieApi.MovieDb.TV;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.Services.AddSerilog((_, config) => config
        .ReadFrom.Configuration(builder.Configuration)
        .WriteTo.Console());

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("DefaultConnection is not configured.");

    var bearerToken = builder.Configuration["Tmdb:BearerToken"]
        ?? throw new InvalidOperationException("Tmdb:BearerToken is not configured.");

    // TMDb API clients
    MovieDbFactory.RegisterSettings(bearerToken);
    builder.Services.AddSingleton(_ => MovieDbFactory.Create<IApiMovieRequest>().Value);
    builder.Services.AddSingleton(_ => MovieDbFactory.Create<IApiTVShowRequest>().Value);
    builder.Services.AddSingleton(_ => MovieDbFactory.Create<IApiPeopleRequest>().Value);
    builder.Services.AddSingleton(_ => MovieDbFactory.Create<IApiDiscoverRequest>().Value);
    builder.Services.AddSingleton(new TmdbDirectClient(bearerToken));

    // Database context
    builder.Services.AddDbContext<TmdbSyncDbContext>(options =>
        options.UseNpgsql(connectionString));

    // Infrastructure
    builder.Services.AddSingleton<TmdbRateLimiter>();
    builder.Services.AddScoped<CheckpointService>();
    builder.Services.AddScoped<FailureTracker>();

    // Sync services
    builder.Services.AddScoped<MovieFullSync>();
    builder.Services.AddScoped<TvSeriesFullSync>();
    builder.Services.AddScoped<PersonSync>();
    builder.Services.AddHostedService<SyncWorker>();

    await builder.Build().RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
