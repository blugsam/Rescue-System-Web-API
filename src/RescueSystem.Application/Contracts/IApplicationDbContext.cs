using RescueSystem.Domain.Entities.Users;
using RescueSystem.Domain.Entities.Bracelets;

namespace RescueSystem.Application.Contracts;

public interface IApplicationDbContext
{
    IQueryable<User> Users { get; }
    IQueryable<Bracelet> Bracelets { get; }
}
