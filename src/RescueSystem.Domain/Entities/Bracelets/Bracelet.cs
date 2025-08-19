using RescueSystem.Domain.Entities.Alerts;
using RescueSystem.Contracts.Contracts.Enums;
using System.Collections.ObjectModel;

namespace RescueSystem.Domain.Entities.Bracelets;

public class Bracelet
{
    public Guid Id { get; private set; }
    public string SerialNumber { get; private set; }
    public DateTime RegistrationInSystemDate { get; private set; }
    public DateTime? LastRepairDate { get; private set; }
    public BraceletStatus Status { get; private set; }
    public Guid? UserId { get; private set; }
    private ICollection<Alert> Alerts { get; set; } = new List<Alert>();

    private readonly Collection<Guid> _alertIds = new();
    public IReadOnlyCollection<Guid> AlertIds => _alertIds;

    public Bracelet()
    {

    }

    private Bracelet(Guid id, string serialNumber, DateTime registrationDate, BraceletStatus status, Guid? userId)
    {
        Id = id;
        SerialNumber = serialNumber;
        RegistrationInSystemDate = registrationDate;
        LastRepairDate = null;
        Status = status;
        UserId = userId;
        Alerts = new List<Alert>();
        _alertIds = new Collection<Guid>();
    }

    public static Bracelet Create(string serialNumber, Guid? userId = null)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(serialNumber);

        return new Bracelet(Guid.NewGuid(), serialNumber, DateTime.UtcNow, BraceletStatus.Inactive, userId);
    }
}
