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
    // Database context
    builder.Services.AddDbContext<TmdbSyncDbContext>(options =>
        options.UseNpgsql(connectionString));

    // Infrastructure
    builder.Services.AddSingleton<TmdbRateLimiter>();
    builder.Services.AddSingleton(sp =>
        new TmdbDirectClient(bearerToken, sp.GetRequiredService<TmdbRateLimiter>()));
    builder.Services.AddScoped<CheckpointService>();
    builder.Services.AddScoped<FailureTracker>();

    // Sync services
    builder.Services.AddScoped<MovieFullSync>();
    builder.Services.AddScoped<TvSeriesFullSync>();
    builder.Services.AddScoped<PersonSync>();
    builder.Services.AddHostedService<SyncWorker>();

    var host = builder.Build();

    // Create sync-only tables not covered by API migrations
    using (var scope = host.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<TmdbSyncDbContext>();
        await db.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS sync_checkpoints (
                sync_type varchar(50) PRIMARY KEY,
                last_page int NOT NULL,
                total_pages int NOT NULL,
                updated_at timestamptz NOT NULL
            );
            CREATE TABLE IF NOT EXISTS sync_failures (
                id bigserial PRIMARY KEY,
                sync_type varchar(50) NOT NULL,
                tmdb_id int NOT NULL,
                error_message varchar(2000) NOT NULL,
                retry_count int NOT NULL DEFAULT 0,
                failed_at timestamptz NOT NULL,
                resolved_at timestamptz NULL
            );
            CREATE INDEX IF NOT EXISTS ix_sync_failures_lookup
                ON sync_failures(sync_type, tmdb_id, resolved_at);
        ");
    }

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
