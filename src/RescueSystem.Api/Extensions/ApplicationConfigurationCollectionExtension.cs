using Serilog;

namespace RescueSystem.Api.Extensions;

public static class ApplicationConfigurationCollectionExtension
{
    public static IApplicationBuilder AddApplicationConfiguration(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAuthorization();

        return app;
    }
}
