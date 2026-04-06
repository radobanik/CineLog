using Amazon.S3;
using CineLog.Application.Common;
using CineLog.Application.Features.Auth;
using CineLog.Domain.Entities;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using CineLog.Infrastructure.BlobStorage;
using CineLog.Infrastructure.Caching;
using CineLog.Infrastructure.Data;
using CineLog.Infrastructure.Notifications;
using CineLog.Infrastructure.Repositories;
using CineLog.Infrastructure.Search;
using CineLog.Infrastructure.Services;
using EFCoreSecondLevelCacheInterceptor;
using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace CineLog.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        var redisConnectionString = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Connection string 'Redis' is not configured.");

        // Redis
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "CineLog:";
        });

        // EF Core second-level cache, backed by IDistributedCache to Redis
        services.AddScoped<DistributedEFCacheServiceProvider>();

        services.AddEFSecondLevelCache(options =>
            options
                .UseCustomCacheProvider<DistributedEFCacheServiceProvider>()
                .UseCacheKeyPrefix("CineLog_")
                .ConfigureLogging(false)
                .SkipCachingResults(r =>
                    r.Value is null ||
                    (r.Value is EFTableRows rows && rows.RowsCount == 0)));

        // AppDbContext
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                npgsql.EnableRetryOnFailure(3);
            });
        });

        // ASP.NET Core Identity
        services.AddIdentityCore<User>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMovieRepository, MovieRepository>();
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();

        // Application-level cache
        services.AddScoped<ICacheService, RedisCacheService>();

        // Elasticsearch
        var esUri = configuration["Elasticsearch:Uri"] ?? "http://localhost:9200";
        var esSettings = new ElasticsearchClientSettings(new Uri(esUri));
        services.AddSingleton(new ElasticsearchClient(esSettings));
        services.AddScoped<IElasticSearchService, ElasticSearchService>();

        // Blob storage (MinIO)
        services.Configure<BlobStorageOptions>(configuration.GetSection("BlobStorage"));
        var blobOptions = configuration.GetSection("BlobStorage").Get<BlobStorageOptions>() ?? new BlobStorageOptions();
        services.AddSingleton<IAmazonS3>(new AmazonS3Client(
            blobOptions.AccessKey,
            blobOptions.SecretKey,
            new AmazonS3Config { ServiceURL = blobOptions.Endpoint, ForcePathStyle = true }));
        services.AddScoped<IBlobStorageService, S3BlobStorageService>();

        // JWT
        services.AddScoped<IJwtService, JwtService>();

        // SignalR and Notification service
        services.AddSignalR();
        services.AddScoped<INotificationService, SignalRNotificationService>();

        return services;
    }
}
