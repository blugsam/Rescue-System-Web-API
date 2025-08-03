using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Contracts.Contracts.Responses;
using RescueSystem.Domain.Entities;

namespace RescueSystem.Application.Mapping;

public static class UserMapping
{
    public static User ToEntity(this CreateUserRequestDto dto)
    {
        return new User
        {
            FullName = dto.FullName,
            DateOfBirth = dto.DateOfBirth.HasValue ? dto.DateOfBirth.Value : default,
            MedicalNotes = dto.MedicalNotes,
            EmergencyContact = dto.EmergencyContact
        };
    }

    public static void UpdateEntity(this User entity, UpdateUserRequestDto dto)
    {
        entity.FullName = dto.FullName;
        entity.DateOfBirth = dto.DateOfBirth.HasValue ? dto.DateOfBirth.Value : default;
        entity.MedicalNotes = dto.MedicalNotes;
        entity.EmergencyContact = dto.EmergencyContact;
    }

    public static UserSummaryDto ToSummaryDto(this User entity)
    {
        return new UserSummaryDto
        {
            Id = entity.Id,
            FullName = entity.FullName
        };
    }

    public static UserDetailsDto ToDetailsDto(this User entity)
    {
        return new UserDetailsDto
        {
            Id = entity.Id,
            FullName = entity.FullName,
            DateOfBirth = entity.DateOfBirth,
            MedicalNotes = entity.MedicalNotes,
            EmergencyContact = entity.EmergencyContact
        };
    }
}
