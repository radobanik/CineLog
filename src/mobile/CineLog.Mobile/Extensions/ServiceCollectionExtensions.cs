using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Navigation;
using CineLog.Mobile.Pages.Auth;
using CineLog.Mobile.Pages.MainPages;
using CineLog.Mobile.Pages.Movies;
using CineLog.Mobile.Services;
using Microsoft.Extensions.DependencyInjection;
#if ANDROID
using CineLog.Mobile.Platforms.Android.Services;
#endif

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
#if ANDROID
        services.AddSingleton<IFcmService, FcmService>();
#endif
        services.AddSingleton<IMovieNavigationContext, MovieNavigationContext>();
        services.AddSingleton<IWatchListNavigationContext, WatchListNavigationContext>();
        services.AddSingleton<IMovieDetailNavigationContext, MovieDetailNavigationContext>();
        services.AddSingleton<IReviewsNavigationContext, ReviewsNavigationContext>();
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
        services.AddTransient<MovieDetailPage>();
        services.AddTransient<AddToWatchlistPage>();
        services.AddTransient<MovieReviewsPage>();
        services.AddTransient<SearchPage>();
        services.AddTransient<LogPage>();
        services.AddTransient<WatchListsPage>();
        services.AddTransient<MovieWatchListPage>();
        services.AddTransient<ProfilePage>();
        services.AddTransient<EditProfilePage>();
        return services;
    }
}
