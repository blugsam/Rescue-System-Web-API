using RescueSystem.Domain.Entities.Users;
using RescueSystem.Domain.Entities.Bracelets;
using RescueSystem.Domain.Entities.Alerts;

namespace RescueSystem.Application.Contracts;

public interface IApplicationDbContext
{
    IQueryable<User> Users { get; }
    IQueryable<Bracelet> Bracelets { get; }
    IQueryable<Alert> Alerts { get; }
}
