using Microsoft.EntityFrameworkCore;
using RescueSystem.Domain.Entities.Health;
using RescueSystem.Domain.Interfaces;
using System.Linq.Expressions;

namespace RescueSystem.Infrastructure.Repositories;

public class HealthProfileThresholdsRepository : IHealthProfileThresholdsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public HealthProfileThresholdsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthProfileThresholds?> FindAsync(Expression<Func<HealthProfileThresholds, bool>> predicate)
    {
        return await _dbContext.HealthProfileThresholds.FirstOrDefaultAsync(predicate);
    }
}