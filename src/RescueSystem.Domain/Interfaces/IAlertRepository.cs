using RescueSystem.Domain.Entities.Alerts;

namespace RescueSystem.Domain.Interfaces;

public interface IAlertRepository
{
    Task<Alert?> GetByIdAsync(Guid id);
    Task<IEnumerable<Alert>> GetAllAsync();
    Task AddAsync(Alert alert);
    void Remove(Alert alert);
    void RemoveById(Guid id);
    Task<int> SaveChangesAsync();
}
