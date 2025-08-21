using RescueSystem.Domain.Entities.Users;

namespace RescueSystem.Application.Contracts;

public interface IApplicationDbContext
{
    IQueryable<User> Users { get; }
}
