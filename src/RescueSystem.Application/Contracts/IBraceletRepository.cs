using RescueSystem.Domain.Entities.Bracelets;

namespace RescueSystem.Application.Contracts;

public interface IBraceletRepository : IGenericRepository<Bracelet>
{
    Task<Bracelet?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default);
}