using RescueSystem.Domain.Entities.Users;

namespace RescueSystem.Application.Contracts;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByBraceletIdAsync(Guid braceletId, CancellationToken cancellationToken = default);
}