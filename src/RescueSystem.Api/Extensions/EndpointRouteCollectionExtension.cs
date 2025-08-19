using RescueSystem.Api.Hubs;

namespace RescueSystem.Api.Extensions;

public static class EndpointRouteCollectionExtension
{
    public static IEndpointRouteBuilder AddRouteConfiguration(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", () => "Rescue System API host successfully started.");

        app.MapControllers();
        app.MapHub<AlertHub>("/alert-hub");

        return app;
    }
}
