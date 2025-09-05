namespace RescueSystem.Api.Extensions;

public static class ServiceDescriptorsCollectionExtension
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddLogging();
        
        return services;
    }
}
