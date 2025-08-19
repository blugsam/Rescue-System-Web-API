using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using RescueSystem.Domain.Interfaces;
using RescueSystem.Infrastructure.Repositories;
using RescueSystem.Infrastructure.Data;

namespace RescueSystem.Infrastructure.Extensions;

public static class InfrastructureServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
        });

        services.AddScoped<IBraceletRepository, BraceletRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAlertRepository, AlertRepository>();
        services.AddScoped<IHealthProfileThresholdsRepository, HealthProfileThresholdsRepository>();

        return services;
    }
}

