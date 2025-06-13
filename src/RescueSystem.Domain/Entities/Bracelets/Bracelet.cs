using RescueSystem.Domain.Entities.Alerts;
using RescueSystem.Contracts.Contracts.Enums;

namespace RescueSystem.Domain.Entities.Bracelets;

public class Bracelet
{
    public Guid Id { get; set; }
    public required string SerialNumber { get; set; }
    public BraceletStatus Status { get; set; }

    public Guid? UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
}