using RescueSystem.Domain.Entities.Alerts;
using RescueSystem.Contracts.Contracts.Enums;

namespace RescueSystem.Domain.Entities.Bracelets;

public class Bracelet
{
    public Guid Id { get; private set; }
    public string SerialNumber { get; private set; }
    public DateTime RegistrationInSystemDate { get; private set; }
    public DateTime? LastRepairDate { get; private set; }
    public BraceletStatus Status { get; private set; }
    public Guid? UserId { get; private set; }

    private readonly HashSet<Guid> _alertIds = new();

    private ICollection<Alert> Alerts { get; set; } = new List<Alert>();

    public IReadOnlySet<Guid> AlertIds => _alertIds;

    public Bracelet()
    {

    }

    private Bracelet(Guid id, string serialNumber, DateTime registrationDate, DateTime? lastRepairDate, BraceletStatus status, Guid? userId)
    {
        Id = id;
        SerialNumber = serialNumber;
        RegistrationInSystemDate = registrationDate;
        LastRepairDate = lastRepairDate;
        Status = status;
        UserId = userId;
    }

    public static Bracelet Create(string serialNumber, DateTime? lastRepairDate = null , Guid? userId = null)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(serialNumber);

        return new Bracelet(Guid.NewGuid(), serialNumber, DateTime.UtcNow, lastRepairDate, BraceletStatus.Inactive, userId);
    }

    public void AssignUser(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (UserId.HasValue && UserId.Value != userId)
            throw new InvalidOperationException($"Bracelet {Id} already assigned to user {UserId}");

        UserId = userId;
        if (Status != BraceletStatus.Active)
            Status = BraceletStatus.Active;
    }

    public void UnassignUser(Guid expectedUserId)
    {
        if (!UserId.HasValue)
            throw new InvalidOperationException($"Bracelet {Id} is not assigned");

        if (UserId.Value != expectedUserId)
            throw new InvalidOperationException($"Bracelet {Id} assigned to another user {UserId}, not {expectedUserId}");

        UserId = null;
        if (Status != BraceletStatus.Inactive)
            Status = BraceletStatus.Inactive;
    }

    public void ChangeStatus(BraceletStatus newStatus)
    {
        if (newStatus == Status)
            return;

        Status = newStatus;
    }
}
