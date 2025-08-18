using RescueSystem.Api.Services;
using RescueSystem.Application.Interfaces;

namespace RescueSystem.Api.Extensions;

public static class PresentationServiceCollectionExtension
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddLogging();
        services.AddSignalR();
        services.AddScoped<IAlertNotifier, SignalRAlertNotifier>();

        return services;
    }
}
