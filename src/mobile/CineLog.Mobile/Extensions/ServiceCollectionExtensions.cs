using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Navigation;
using CineLog.Mobile.Pages.Auth;
using CineLog.Mobile.Pages.MainPages;
using CineLog.Mobile.Pages.Movies;
using CineLog.Mobile.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CineLog.Mobile.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCineLogMobile(this IServiceCollection services)
    {
        services.AddMauiServices();
        services.AddPages();
        return services;
    }

    private static IServiceCollection AddMauiServices(this IServiceCollection services)
    {
        services.AddSingleton<ISecureStorageService, MauiSecureStorageService>();
        services.AddSingleton<INavigationService, ShellNavigationService>();
        services.AddSingleton<IAlertService, MauiAlertService>();
        services.AddSingleton<IKeyboardService, KeyboardService>();
        return services;
    }

    private static IServiceCollection AddPages(this IServiceCollection services)
    {
        services.AddTransient<LoginPage>();
        services.AddTransient<RegisterPage>();
        services.AddTransient<HomePage>();
        services.AddTransient<MoviesCategoryPage>();
        services.AddTransient<SearchPage>();
        services.AddTransient<LogPage>();
        services.AddTransient<ListsPage>();
        services.AddTransient<ProfilePage>();
        return services;
    }
}
