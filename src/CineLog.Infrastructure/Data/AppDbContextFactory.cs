using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace CineLog.Infrastructure.Data;

/// <summary>
/// Used by EF Core tooling (dotnet ef migrations) at design time.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Database=cinelog_dev;Username=postgres;Password=postgres";

        // Build minimal services for the interceptor
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddStackExchangeRedisCache(o => o.Configuration = "localhost:6379");
        services.AddScoped<DistributedEFCacheServiceProvider>();
        services.AddEFSecondLevelCache(o => o.UseCustomCacheProvider<DistributedEFCacheServiceProvider>());

        var sp = services.BuildServiceProvider();
        var interceptor = sp.GetRequiredService<SecondLevelCacheInterceptor>();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString, npgsql =>
            npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));

        return new AppDbContext(optionsBuilder.Options, interceptor);
    }
}
