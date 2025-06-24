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
        return services;
    }
}
