using Microsoft.EntityFrameworkCore;
using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Entities.Alerts;
using RescueSystem.Domain.Entities.Bracelets;
using RescueSystem.Domain.Entities.Health;

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
        public DbSet<AlertValidationError> AlertValidationErrors { get; set; }
        public DbSet<HealthMetric> HealthMetrics { get; set; }
        public DbSet<HealthProfileThresholds> HealthProfileThresholds { get; set; }

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

                entity.HasOne(u => u.HealthProfile)
                      .WithMany(h => h.Users)
                      .HasForeignKey(u => u.HealthProfileId)
                      .OnDelete(DeleteBehavior.SetNull);
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

                entity.Property(a => a.QualityLevel)
                      .HasConversion<string>()
                      .IsRequired();

                entity.HasMany(a => a.ValidationErrors)
                      .WithOne(ve => ve.Alert)
                      .HasForeignKey(ve => ve.AlertId)
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

            modelBuilder.Entity<AlertValidationError>(entity =>
            {
                entity.HasKey(ve => ve.Id);
                entity.Property(ve => ve.ErrorMessage).IsRequired();
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

            modelBuilder.Entity<HealthProfileThresholds>(entity =>
            {
                entity.HasKey(h => h.Id);

                entity.Property(h => h.ProfileName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.HasIndex(h => h.ProfileName)
                      .IsUnique();

                entity.HasData(
                    new HealthProfileThresholds
                    {
                        Id = Guid.Parse("a66ca370-bf93-4c12-be9a-086b98136eea"),
                        ProfileName = "Default",
                        HighPulseThreshold = 160,
                        LowPulseThreshold = 50,
                        HighTempThreshold = 38.5,
                        LowTempThreshold = 35.0
                    },
                    new HealthProfileThresholds
                    {
                        Id = Guid.Parse("c1464740-bca9-4289-8415-2e8eadf6d623"),
                        ProfileName = "Hypertensive",
                        HighPulseThreshold = 140,
                        LowPulseThreshold = 55,
                        HighTempThreshold = 38.5,
                        LowTempThreshold = 35.0
                    },
                    new HealthProfileThresholds
                    {
                        Id = Guid.Parse("4d74cc16-2a76-4772-9643-e82cb122c898"),
                        ProfileName = "Athlete",
                        HighPulseThreshold = 180,
                        LowPulseThreshold = 40,
                        HighTempThreshold = 38.5,
                        LowTempThreshold = 35.0
                    }
                );
            });
        }

    }
}