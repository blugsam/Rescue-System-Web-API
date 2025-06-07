using RescueSystem.Domain.Entities.Alerts;

namespace RescueSystem.Domain.Entities.Bracelets
{
    public class Bracelet
    {
        public Guid Id { get; set; }
        public required string SerialNumber { get; set; }
        public BraceletStatus Status { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
    }
}