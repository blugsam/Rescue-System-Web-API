using Serilog;
using RescueSystem.Api.Hubs;
using RescueSystem.Api.Extensions;
using RescueSystem.Infrastructure.Extensions;
using RescueSystem.Application.Extensions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;

    builder.Host.UseSerilog((ctx, services, lc) =>
        lc.ReadFrom.Configuration(ctx.Configuration)
          .ReadFrom.Services(services)
          .Enrich.FromLogContext());

    builder.Services.AddPresentation();
    builder.Services.AddInfrastructure(configuration);
    builder.Services.AddApplication();

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.MapGet("/", () => "Rescue System API host successfully started.");

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHub<AlertHub>("/alert-hub");
    app.Run();
}
catch(Exception exception)
{
    Log.Fatal(exception, "App terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}