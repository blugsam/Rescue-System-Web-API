using Microsoft.EntityFrameworkCore;
using RescueSystem.Application.Validation;
using RescueSystem.Infrastructure;

using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddValidatorsFromAssemblyContaining<CreateAlertRequestValidator>();

builder.Services.AddDbContext<RescueDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsAssembly(typeof(RescueDbContext).Assembly.FullName);
    });
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();