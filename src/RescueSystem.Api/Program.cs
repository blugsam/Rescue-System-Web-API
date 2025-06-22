using FluentValidation;
using Microsoft.Extensions.Configuration;
using RescueSystem.Api.Extensions;
using RescueSystem.Api.Hubs;
using RescueSystem.Api.Services;
using RescueSystem.Application.Interfaces;
using RescueSystem.Application.Validation;
using RescueSystem.Infrastructure.Extensions;
using Serilog;

//var configuration = new ConfigurationBuilder()
//    .SetBasePath(Directory.GetCurrentDirectory())
//    .AddJsonFile("appsettings.json")
//    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
//    .Build();

Log.Logger = new LoggerConfiguration()
    //.ReadFrom.Configuration(configuration)
    .WriteTo.Console()
    .CreateLogger();
Log.Information("Starting Rescue System API host.");

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddSerilog();

    builder.Services.AddPresentation();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();

    builder.Services.AddValidatorsFromAssemblyContaining<CreateAlertRequestValidator>();

    builder.Services.AddScoped<IAlertNotifier, SignalRAlertNotifier>();

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.MapGet("/", () => "Rescue System API host succesfuly started.");

    app.UseApiExceptionHandler();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
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