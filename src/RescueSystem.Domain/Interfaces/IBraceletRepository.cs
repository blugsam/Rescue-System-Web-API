using RescueSystem.Domain.Entities.Bracelets;
using System.Linq.Expressions;

namespace RescueSystem.Domain.Interfaces;

public interface IBraceletRepository
{
    Task<Bracelet?> GetByIdWithUserAsync(Guid id);
    Task<Bracelet?> GetBraceletBySerialNumber(string serialNumber);
    Task<IEnumerable<Bracelet>> FindAsync(Expression<Func<Bracelet, bool>> predicate);
    Task<IEnumerable<Bracelet>> GetAllAsync();
    Task AddAsync(Bracelet entity);
    void Remove(Bracelet entity);
    Task<int> SaveChangesAsync();
    Task<Bracelet?> GetByIdAsync(Guid id);
}
