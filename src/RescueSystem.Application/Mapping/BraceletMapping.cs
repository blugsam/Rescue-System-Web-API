using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Contracts.Contracts.Responses;
using RescueSystem.Domain.Entities.Bracelets;

namespace RescueSystem.Application.Mapping;

public static class BraceletMapping
{
    public static Bracelet ToEntity(this CreateBraceletRequestDto dto)
    {
        return new Bracelet
        {
            SerialNumber = dto.SerialNumber
        };
    }

    public static BraceletDto ToDto(this Bracelet entity)
    {
        return new BraceletDto
        {
            Id = entity.Id,
            SerialNumber = entity.SerialNumber,
            Status = entity.Status.ToString()
        };
    }

    public static BraceletDetailsDto ToDetailsDto(this Bracelet entity)
    {
        return new BraceletDetailsDto
        {
            Id = entity.Id,
            SerialNumber = entity.SerialNumber,
            Status = entity.Status.ToString(),
            AssignedUser = entity.User?.ToSummaryDto()
        };
    }
}