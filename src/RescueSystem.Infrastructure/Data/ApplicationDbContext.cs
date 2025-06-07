using Microsoft.EntityFrameworkCore;
using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Entities.Bracelets;
using RescueSystem.Domain.Entities.Alerts;

namespace RescueSystem.Infrastructure
{
    public class RescueDbContext : DbContext
    {
        public RescueDbContext(DbContextOptions<RescueDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Bracelet> Bracelets { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<AlertTrigger> AlertTriggers { get; set; }
        public DbSet<HealthMetric> HealthMetrics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.FullName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(u => u.DateOfBirth)
                      .IsRequired();

                entity.HasOne(u => u.Bracelet)
                      .WithOne(b => b.User)
                      .HasForeignKey<Bracelet>(b => b.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Bracelet>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.SerialNumber)
                      .IsRequired()
                      .HasMaxLength(50);
                entity.HasIndex(b => b.SerialNumber)
                      .IsUnique();

                entity.Property(b => b.Status)
                      .HasConversion<string>()
                      .IsRequired();

                entity.Property(b => b.UserId)
                      .IsRequired();

                entity.HasMany(b => b.Alerts)
                      .WithOne(a => a.Bracelet)
                      .HasForeignKey(a => a.BraceletId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Alert>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Status)
                      .HasConversion<string>()
                      .IsRequired();

                entity.Property(a => a.Timestamp)
                      .IsRequired();

                entity.Property(a => a.Latitude)
                      .IsRequired();
                entity.Property(a => a.Longitude)
                      .IsRequired();

                entity.HasMany(a => a.Triggers)
                      .WithOne(t => t.Alert)
                      .HasForeignKey(t => t.AlertId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.HealthMetric)
                      .WithOne(hm => hm.Alert)
                      .HasForeignKey<HealthMetric>(hm => hm.AlertId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AlertTrigger>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Type)
                      .HasConversion<string>()
                      .IsRequired();

                entity.HasIndex(t => new { t.AlertId, t.Type })
                      .IsUnique();
            });

            modelBuilder.Entity<HealthMetric>(entity =>
            {
                entity.HasKey(hm => hm.Id);

                entity.Property(hm => hm.AlertId)
                      .IsRequired();

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_HealthMetric_Pulse", "\"Pulse\" IS NULL OR (\"Pulse\" >= 30 AND \"Pulse\" <= 250)");
                    t.HasCheckConstraint("CK_HealthMetric_Temp", "\"BodyTemperature\" IS NULL OR (\"BodyTemperature\" >= 30 AND \"BodyTemperature\" <= 45)");
                });
            });
        }

    }
}