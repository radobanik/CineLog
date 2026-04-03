using System.Threading.RateLimiting;
using Microsoft.EntityFrameworkCore;
using CineLog.Application.Common;
using CineLog.Application.Extensions;
using CineLog.Api.Exceptions;
using CineLog.Api.Extensions;
using CineLog.Api.Services;
using CineLog.Infrastructure.Data;
using CineLog.Infrastructure.Extensions;
using CineLog.Infrastructure.Notifications;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using Serilog.Debugging;

var builder = WebApplication.CreateBuilder(args);

SelfLog.Enable(Console.Error);

// Serilog
builder.Host.UseSerilog((ctx, config) =>
{
    var seqUrl = ctx.Configuration["Serilog:Seq:ServerUrl"] ?? "http://localhost:5341";
    config.ReadFrom.Configuration(ctx.Configuration)
          .Enrich.FromLogContext()
          .Enrich.WithThreadId()
          .Enrich.WithMachineName()
          .WriteTo.Console()
          .WriteTo.File("logs/cinelog-.txt", rollingInterval: Serilog.RollingInterval.Day)
          .WriteTo.Seq(seqUrl);
});

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

// Auth
builder.Services.AddJwtAuth(builder.Configuration);
builder.Services.AddAuthorization();

// MVC
builder.Services.AddControllers();

// Exception handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// HTTP context + current user
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Swagger
builder.Services.AddSwaggerWithJwt();

// Connection strings
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? string.Empty;

builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "postgres")
    .AddRedis(redisConnectionString, name: "redis");

// Rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("auth", limiterOptions =>
    {
        limiterOptions.PermitLimit = 10;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();

// Auto-migrate
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Database migration or seeding skipped (database may not be available).");
    }
}

// Elasticsearch
using (var scope = app.Services.CreateScope())
{
    try
    {
        var es = scope.ServiceProvider.GetRequiredService<IElasticSearchService>();
        await es.EnsureIndicesExistAsync();

        if (await es.CountMoviesAsync() == 0)
        {
            var sender = scope.ServiceProvider.GetRequiredService<MediatR.ISender>();
            await sender.Send(new CineLog.Application.Features.Movies.ReindexMovies.ReindexMoviesCommand());
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Elasticsearch setup skipped (Elasticsearch may not be available).");
    }
}

// Middleware pipeline
app.UseExceptionHandler();
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");

app.Run();

public partial class Program { }
