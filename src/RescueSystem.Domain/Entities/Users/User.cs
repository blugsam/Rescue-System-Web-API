namespace RescueSystem.Domain.Entities.Users;

public class User
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public DateTime RegistrationDate { get; private set; }
    public string? MedicalNotes { get; private set; }
    public string? EmergencyContact { get; private set; }
    public Guid? BraceletId { get; private set; }

    public User()
    {

    }

    private User(Guid id, string fullName, DateOnly dateOfBirth, DateTime registrationDate,
        string? medicalNotes, string? emergencyContact, Guid? braceletId)
    {
        Id = id;
        FullName = fullName;
        DateOfBirth = dateOfBirth;
        RegistrationDate = registrationDate;
        MedicalNotes = medicalNotes ?? string.Empty;
        EmergencyContact = emergencyContact ?? string.Empty;
        BraceletId = braceletId;
    }

    public static User Create(string fullName, DateOnly dateOfBirth, string? medicalNotes,
        string? emergencyContact, Guid? braceletId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);

        return new User(Guid.NewGuid(), fullName, dateOfBirth, DateTime.UtcNow, medicalNotes, emergencyContact, braceletId);
    }

    public void ChangeName(string newFullName)
    {
        ArgumentException.ThrowIfNullOrEmpty(newFullName);

        if (newFullName == FullName)
            return;

        FullName = newFullName;
    }

    public void ChangeDateOfBirth(DateOnly newDateOfBirth)
    {
        if (newDateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentOutOfRangeException(nameof(newDateOfBirth), "User's date of birth cannot be in the future");

        DateOfBirth = newDateOfBirth;
    }

    public void ChangeMedicalNotes(string newMedicalNotes)
    {
        if (newMedicalNotes == MedicalNotes)
            return;

        MedicalNotes = newMedicalNotes ?? string.Empty;
    }

    public void ChangeEmergencyContacts(string newEmergencyContact)
    {
        if (newEmergencyContact == EmergencyContact)
            return;

        EmergencyContact = newEmergencyContact ?? string.Empty;
    }

    public void AttachBracelet(Guid attachingBraceletGuid)
    {
        if (attachingBraceletGuid == Guid.Empty)
            throw new ArgumentException("Bracelet cannot be empty", nameof(attachingBraceletGuid));

        BraceletId = attachingBraceletGuid;
    }

    public void DetachBracelet()
    {
        if (BraceletId is null)
            throw new InvalidOperationException("User does not have an attached bracelet.");

        BraceletId = null;
    }
}