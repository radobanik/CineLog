using System.Net.Http.Json;
using System.Text.Json;
using CineLog.Application.Features.Auth;
using CineLog.Domain.Entities;
using CineLog.Domain.Enums;
using CineLog.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace CineLog.IntegrationTests.Infrastructure;

public class CineLogWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .Build();

    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    async Task IAsyncLifetime.InitializeAsync()
        => await _postgres.StartAsync();

    async Task IAsyncLifetime.DisposeAsync()
        => await _postgres.DisposeAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var toRemove = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                    d.ServiceType == typeof(AppDbContext))
                .ToList();
            foreach (var d in toRemove) services.Remove(d);

            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.UseNpgsql(_postgres.GetConnectionString(), npgsql =>
                    npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            });

            services.RemoveAll<IDistributedCache>();
            services.AddDistributedMemoryCache();
        });

        builder.UseEnvironment("Testing");
    }

    public HttpClient CreateAuthenticatedClient(string token)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    /// <summary>Register a fresh user and return (token, userId).</summary>
    public async Task<(string Token, Guid UserId)> RegisterAsync(
        string? email = null, string? username = null, string password = "Password123!")
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            Username = username ?? $"u{Guid.NewGuid():N}"[..12],
            Email    = email    ?? $"{Guid.NewGuid()}@test.com",
            Password = password
        });
        response.EnsureSuccessStatusCode();
        var auth = JsonSerializer.Deserialize<AuthResponse>(
            await response.Content.ReadAsStringAsync(), JsonOpts)!;
        return (auth.Token, auth.UserId);
    }

    /// <summary>Login an existing user and return the token.</summary>
    public async Task<string> LoginAsync(string email, string password)
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password });
        response.EnsureSuccessStatusCode();
        var auth = JsonSerializer.Deserialize<AuthResponse>(
            await response.Content.ReadAsStringAsync(), JsonOpts)!;
        return auth.Token;
    }

    /// <summary>Login as seeded admin (alice@cinelog.dev / Admin1234!).</summary>
    public Task<string> LoginAsAdminAsync() => LoginAsync("alice@cinelog.dev", "Admin1234!");

    /// <summary>Seed a movie directly via the DB context.</summary>
    public async Task<Guid> SeedMovieAsync(string title = "Test Movie", MovieType type = MovieType.Movie)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var movie = Movie.Create(Random.Shared.Next(1, int.MaxValue), title, type);
        db.Movies.Add(movie);
        await db.SaveChangesAsync();
        return movie.Id;
    }
}
