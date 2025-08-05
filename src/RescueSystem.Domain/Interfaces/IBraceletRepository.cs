using RescueSystem.Domain.Entities.Bracelets;

namespace RescueSystem.Domain.Interfaces;

public interface IBraceletRepository
{
    Task<Bracelet> GetByIdWithUserAsync(Guid id);
}
