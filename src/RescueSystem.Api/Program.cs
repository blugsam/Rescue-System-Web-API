using RescueSystem.Api.Extensions;
using RescueSystem.Api.Hubs;
using RescueSystem.Api.Services;
using RescueSystem.Application.Interfaces;
using RescueSystem.Application.Validation;
using RescueSystem.Infrastructure.Extensions;
using Serilog;
using FluentValidation;

try
{
    var builder = WebApplication.CreateBuilder(args).AddSerilogLogging();

    builder.Host.UseSerilog((ctx, services, lc) =>
        lc.ReadFrom.Configuration(ctx.Configuration)
          .ReadFrom.Services(services)
          .Enrich.FromLogContext());

    builder.Services.AddPresentation();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();
    builder.Services.AddValidatorsFromAssemblyContaining<CreateAlertRequestValidator>();
    builder.Services.AddScoped<IAlertNotifier, SignalRAlertNotifier>();

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.MapGet("/", () => "Rescue System API host succesfuly started.");
    app.UseApiExceptionHandler();

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