using CineLog.Mobile.Core.Extensions;
using CineLog.Mobile.Extensions;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CineLog.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureMauiHandlers(handlers =>
            {
#if ANDROID
                handlers.AddHandler<Shell, CineLog.Mobile.Platforms.Android.CustomShellRenderer>();
#elif IOS
                handlers.AddHandler<Shell, CineLog.Mobile.Platforms.iOS.CustomShellRenderer>();
#endif
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("fa-solid-900.ttf", "FASolid");
            });

        var assembly = typeof(MauiProgram).Assembly;

        using var stream = assembly.GetManifestResourceStream("CineLog.Mobile.appsettings.json")!;
        builder.Configuration.AddJsonStream(stream);

        var devStream = assembly.GetManifestResourceStream("CineLog.Mobile.appsettings.Development.json");
        if (devStream is not null)
        {
            builder.Configuration.AddJsonStream(devStream);
            devStream.Dispose();
        }

        var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
            ?? throw new InvalidOperationException("ApiBaseUrl is not configured in appsettings.json");

        builder.Services.AddCineLogCore(apiBaseUrl);
        builder.Services.AddCineLogMobile();

        builder.Services.AddTransient<AppShell>();
        builder.Services.AddTransient<App>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
