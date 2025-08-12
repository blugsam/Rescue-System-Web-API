using RescueSystem.Domain.Entities;

namespace RescueSystem.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<IEnumerable<User>> GetAllAsync();
    Task AddAsync(User user);
    void Remove(User user);
    Task<int> SaveChangesAsync();
}