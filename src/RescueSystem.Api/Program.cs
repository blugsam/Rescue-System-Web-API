using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RescueSystem.Api.Hubs;
using RescueSystem.Api.Services;
using RescueSystem.Application.Interfaces;
using RescueSystem.Application.Services.AlertService;
using RescueSystem.Application.Services.BraceletService;
using RescueSystem.Application.Services.UserService;
using RescueSystem.Application.Validation;
using RescueSystem.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddSignalR();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<RescueDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsAssembly(typeof(RescueDbContext).Assembly.FullName);
    });
});

builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBraceletService, BraceletService>();
builder.Services.AddScoped<IAlertNotifier, SignalRAlertNotifier>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddValidatorsFromAssemblyContaining<CreateAlertRequestValidator>();

var app = builder.Build();

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