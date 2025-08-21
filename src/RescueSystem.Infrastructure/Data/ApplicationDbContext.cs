using Microsoft.EntityFrameworkCore;
using RescueSystem.Domain.Entities.Alerts;
using RescueSystem.Domain.Entities.Bracelets;
using RescueSystem.Domain.Entities.Health;
using RescueSystem.Domain.Entities.Users;

namespace RescueSystem.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Bracelet> Bracelets { get; set; }
    public DbSet<Alert> Alerts { get; set; }
    public DbSet<HealthMetric> HealthMetrics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}