using RescueSystem.Domain.Entities.Bracelets;
using RescueSystem.Domain.Entities.Health;

namespace RescueSystem.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public required string FullName { get; set; }
    public DateOnly DateOfBirth { get; set; }

    public Bracelet? Bracelet { get; set; }

    public Guid? HealthProfileId { get; set; }
    public HealthProfileThresholds? HealthProfile { get; set; }
}