using Microsoft.EntityFrameworkCore;
using RescueSystem.Domain.Entities.Alerts;
using RescueSystem.Domain.Interfaces;

namespace RescueSystem.Infrastructure.Repositories;

public class AlertRepository : IAlertRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AlertRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Alert?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Alerts
            .Include(a => a.Triggers)
            .Include(a => a.ValidationErrors)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Alert>> GetAllAsync()
    {
        return await _dbContext.Alerts
            .Include(a => a.Triggers)
            .Include(a => a.Bracelet)
            .ThenInclude(b => b!.User)
            .ToListAsync();
    }

    public async Task AddAsync(Alert alert)
    {
        await _dbContext.Alerts.AddAsync(alert);
    }

    public void Remove(Alert alert)
    {
        _dbContext.Alerts.Remove(alert);
    }

    public void RemoveById(Guid id)
    {
        var alert = _dbContext.Alerts.Find(id);
        if (alert != null)
        {
            _dbContext.Alerts.Remove(alert);
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}
