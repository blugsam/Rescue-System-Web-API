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

    public void ChangeStatus(BraceletStatus newStatus)
    {
        if (newStatus == Status)
            return;

        Status = newStatus;
    }
}
