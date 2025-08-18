using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using RescueSystem.Application.Services.AlertService;
using RescueSystem.Application.Services.BraceletService;
using RescueSystem.Application.Services.UserService;
using RescueSystem.Application.Validation;

namespace RescueSystem.Application.Extensions;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAlertService, AlertService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBraceletService, BraceletService>();
        services.AddValidatorsFromAssemblyContaining<CreateAlertRequestValidator>();

        return services;
    }
}
