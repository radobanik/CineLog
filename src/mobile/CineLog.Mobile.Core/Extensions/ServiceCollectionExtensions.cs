using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.Core.Infrastructure;
using CineLog.Mobile.Core.Services;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Auth;
using CineLog.Mobile.Core.ViewModels.Common;
using CineLog.Mobile.Core.ViewModels.Home;
using CineLog.Mobile.Core.ViewModels.Profile;
using Microsoft.Extensions.DependencyInjection;

namespace CineLog.Mobile.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCineLogCore(
        this IServiceCollection services,
        string apiBaseUrl)
    {
        services.AddSession();
        services.AddHttpInfrastructure(apiBaseUrl);
        services.AddApiClients();
        services.AddCoreServices();
        services.AddViewModels();
        return services;
    }

    private static IServiceCollection AddSession(this IServiceCollection services)
    {
        services.AddSingleton<ISessionService, SessionService>();
        return services;
    }

    private static IServiceCollection AddHttpInfrastructure(this IServiceCollection services, string apiBaseUrl)
    {
        services.AddTransient<AuthenticatedHttpMessageHandler>();
        services.AddHttpClient("CineLogApi", client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            })
            .AddHttpMessageHandler<AuthenticatedHttpMessageHandler>();
        return services;
    }

    private static IServiceCollection AddApiClients(this IServiceCollection services)
    {
        services.AddTransient<IAuthClient>(sp =>
            new AuthClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("CineLogApi")));

        services.AddTransient<IUsersClient>(sp =>
            new UsersClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("CineLogApi")));

        services.AddTransient<IDashboardClient>(sp =>
            new DashboardClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("CineLogApi")));

        return services;
    }

    private static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IHomeService, HomeService>();
        return services;
    }

    private static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<MoviesCategoryViewModel>();
        services.AddTransient<ProfileViewModel>();
        return services;
    }
}
